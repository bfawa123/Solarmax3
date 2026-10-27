using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	public class CallbackSet
	{
		internal CallbackSet(MetaType metaType)
		{
			if (metaType == null)
			{
				throw new ArgumentNullException("metaType");
			}
			this.metaType = metaType;
		}

		internal MethodInfo this[TypeModel.CallbackType callbackType]
		{
			get
			{
				switch (callbackType)
				{
				case TypeModel.CallbackType.BeforeSerialize:
					return this.beforeSerialize;
				case TypeModel.CallbackType.AfterSerialize:
					return this.afterSerialize;
				case TypeModel.CallbackType.BeforeDeserialize:
					return this.beforeDeserialize;
				case TypeModel.CallbackType.AfterDeserialize:
					return this.afterDeserialize;
				default:
					throw new ArgumentException();
				}
			}
		}

		internal static bool CheckCallbackParameters(TypeModel model, MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			return parameters.Length == 0 || (parameters.Length == 1 && parameters[0].ParameterType == model.MapType(typeof(SerializationContext)));
		}

		private MethodInfo SanityCheckCallback(TypeModel model, MethodInfo callback)
		{
			this.metaType.ThrowIfFrozen();
			if (callback == null)
			{
				return callback;
			}
			if (callback.IsStatic)
			{
				throw new ArgumentException("Callbacks cannot be static", "callback");
			}
			if (callback.ReturnType != model.MapType(typeof(void)) || !CallbackSet.CheckCallbackParameters(model, callback))
			{
				throw CallbackSet.CreateInvalidCallbackSignature(callback);
			}
			return callback;
		}

		internal static Exception CreateInvalidCallbackSignature(MethodInfo method)
		{
			return new NotSupportedException("Invalid callback signature in " + method.DeclaringType.FullName + "." + method.Name);
		}

		public MethodInfo BeforeSerialize
		{
			get
			{
				return this.beforeSerialize;
			}
			set
			{
				this.beforeSerialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo BeforeDeserialize
		{
			get
			{
				return this.beforeDeserialize;
			}
			set
			{
				this.beforeDeserialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo AfterSerialize
		{
			get
			{
				return this.afterSerialize;
			}
			set
			{
				this.afterSerialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo AfterDeserialize
		{
			get
			{
				return this.afterDeserialize;
			}
			set
			{
				this.afterDeserialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public bool NonTrivial
		{
			get
			{
				return this.beforeSerialize != null || this.beforeDeserialize != null || this.afterSerialize != null || this.afterDeserialize != null;
			}
		}

		private readonly MetaType metaType;

		private MethodInfo beforeSerialize;

		private MethodInfo afterSerialize;

		private MethodInfo beforeDeserialize;

		private MethodInfo afterDeserialize;
	}
}
