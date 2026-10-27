using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	internal static class ExtensibleUtil
	{
		internal static IEnumerable<TValue> GetExtendedValues<TValue>(IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			IEnumerator enumerator = ExtensibleUtil.GetExtendedValues(RuntimeTypeModel.Default, typeof(TValue), instance, tag, format, singleton, allowDefinedTag).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					TValue value = (TValue)((object)obj);
					yield return value;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			yield break;
		}

		internal static IEnumerable GetExtendedValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			IExtension extn = instance.GetExtensionObject(false);
			if (extn == null)
			{
				yield break;
			}
			Stream stream = extn.BeginQuery();
			object value = null;
			try
			{
				SerializationContext ctx = new SerializationContext();
				using (ProtoReader reader = new ProtoReader(stream, model, ctx))
				{
					while (model.TryDeserializeAuxiliaryType(reader, format, tag, type, ref value, true, false, false, false) && value != null)
					{
						if (!singleton)
						{
							yield return value;
							value = null;
						}
					}
				}
				if (singleton && value != null)
				{
					yield return value;
				}
			}
			finally
			{
				extn.EndQuery(stream);
			}
			yield break;
		}

		internal static void AppendExtendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			if (extensionObject == null)
			{
				throw new InvalidOperationException("No extension object available; appended data would be lost.");
			}
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, model, null))
				{
					model.TrySerializeAuxiliaryType(protoWriter, null, format, tag, value, false);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		public static void AppendExtendValueTyped<TSource, TValue>(TypeModel model, TSource instance, int tag, DataFormat format, TValue value) where TSource : class, IExtensible
		{
			ExtensibleUtil.AppendExtendValue(model, instance, tag, format, value);
		}
	}
}
