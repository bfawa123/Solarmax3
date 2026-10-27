using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class ParseableSerializer : IProtoSerializer
	{
		private ParseableSerializer(MethodInfo parse)
		{
			this.parse = parse;
		}

		public static ParseableSerializer TryCreate(Type type, TypeModel model)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			MethodInfo method = type.GetMethod("Parse", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, null, new Type[]
			{
				model.MapType(typeof(string))
			}, null);
			if (method != null && method.ReturnType == type)
			{
				if (Helpers.IsValueType(type))
				{
					MethodInfo customToString = ParseableSerializer.GetCustomToString(type);
					if (customToString == null || customToString.ReturnType != model.MapType(typeof(string)))
					{
						return null;
					}
				}
				return new ParseableSerializer(method);
			}
			return null;
		}

		private static MethodInfo GetCustomToString(Type type)
		{
			return type.GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, null, Helpers.EmptyTypes, null);
		}

		public Type ExpectedType
		{
			get
			{
				return this.parse.DeclaringType;
			}
		}

		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public object Read(object value, ProtoReader source)
		{
			return this.parse.Invoke(null, new object[]
			{
				source.ReadString()
			});
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString(value.ToString(), dest);
		}

		private readonly MethodInfo parse;
	}
}
