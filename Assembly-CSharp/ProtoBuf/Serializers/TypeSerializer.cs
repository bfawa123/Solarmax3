using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class TypeSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		public TypeSerializer(TypeModel model, Type forType, int[] fieldNumbers, IProtoSerializer[] serializers, MethodInfo[] baseCtorCallbacks, bool isRootType, bool useConstructor, CallbackSet callbacks, Type constructType, MethodInfo factory)
		{
			Helpers.Sort(fieldNumbers, serializers);
			bool flag = false;
			for (int i = 1; i < fieldNumbers.Length; i++)
			{
				if (fieldNumbers[i] == fieldNumbers[i - 1])
				{
					throw new InvalidOperationException("Duplicate field-number detected; " + fieldNumbers[i].ToString() + " on: " + forType.FullName);
				}
				if (!flag && serializers[i].ExpectedType != forType)
				{
					flag = true;
				}
			}
			this.forType = forType;
			this.factory = factory;
			if (constructType == null)
			{
				constructType = forType;
			}
			else if (!forType.IsAssignableFrom(constructType))
			{
				throw new InvalidOperationException(forType.FullName + " cannot be assigned from " + constructType.FullName);
			}
			this.constructType = constructType;
			this.serializers = serializers;
			this.fieldNumbers = fieldNumbers;
			this.callbacks = callbacks;
			this.isRootType = isRootType;
			this.useConstructor = useConstructor;
			if (baseCtorCallbacks != null && baseCtorCallbacks.Length == 0)
			{
				baseCtorCallbacks = null;
			}
			this.baseCtorCallbacks = baseCtorCallbacks;
			if (Helpers.GetUnderlyingType(forType) != null)
			{
				throw new ArgumentException("Cannot create a TypeSerializer for nullable types", "forType");
			}
			if (model.MapType(TypeSerializer.iextensible).IsAssignableFrom(forType))
			{
				if (forType.IsValueType || !isRootType || flag)
				{
					throw new NotSupportedException("IExtensible is not supported in structs or classes with inheritance");
				}
				this.isExtensible = true;
			}
			this.hasConstructor = (!constructType.IsAbstract && Helpers.GetConstructor(constructType, Helpers.EmptyTypes, true) != null);
			if (constructType != forType && useConstructor && !this.hasConstructor)
			{
				throw new ArgumentException("The supplied default implementation cannot be created: " + constructType.FullName, "constructType");
			}
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			if (this.callbacks != null && this.callbacks[callbackType] != null)
			{
				return true;
			}
			for (int i = 0; i < this.serializers.Length; i++)
			{
				if (this.serializers[i].ExpectedType != this.forType && ((IProtoTypeSerializer)this.serializers[i]).HasCallbacks(callbackType))
				{
					return true;
				}
			}
			return false;
		}

		public Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		private bool CanHaveInheritance
		{
			get
			{
				return (this.forType.IsClass || this.forType.IsInterface) && !this.forType.IsSealed;
			}
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			if (this.callbacks != null)
			{
				this.InvokeCallback(this.callbacks[callbackType], value, context);
			}
			IProtoTypeSerializer protoTypeSerializer = (IProtoTypeSerializer)this.GetMoreSpecificSerializer(value);
			if (protoTypeSerializer != null)
			{
				protoTypeSerializer.Callback(value, callbackType, context);
			}
		}

		private IProtoSerializer GetMoreSpecificSerializer(object value)
		{
			if (!this.CanHaveInheritance)
			{
				return null;
			}
			Type type = value.GetType();
			if (type == this.forType)
			{
				return null;
			}
			for (int i = 0; i < this.serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = this.serializers[i];
				if (protoSerializer.ExpectedType != this.forType && Helpers.IsAssignableFrom(protoSerializer.ExpectedType, type))
				{
					return protoSerializer;
				}
			}
			if (type == this.constructType)
			{
				return null;
			}
			TypeModel.ThrowUnexpectedSubtype(this.forType, type);
			return null;
		}

		public void Write(object value, ProtoWriter dest)
		{
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.BeforeSerialize, dest.Context);
			}
			IProtoSerializer moreSpecificSerializer = this.GetMoreSpecificSerializer(value);
			if (moreSpecificSerializer != null)
			{
				moreSpecificSerializer.Write(value, dest);
			}
			for (int i = 0; i < this.serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = this.serializers[i];
				if (protoSerializer.ExpectedType == this.forType)
				{
					protoSerializer.Write(value, dest);
				}
			}
			if (this.isExtensible)
			{
				ProtoWriter.AppendExtensionData((IExtensible)value, dest);
			}
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.AfterSerialize, dest.Context);
			}
		}

		public object Read(object value, ProtoReader source)
		{
			if (this.isRootType && value != null)
			{
				this.Callback(value, TypeModel.CallbackType.BeforeDeserialize, source.Context);
			}
			int num = 0;
			int num2 = 0;
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				bool flag = false;
				if (num3 < num)
				{
					num2 = (num = 0);
				}
				for (int i = num2; i < this.fieldNumbers.Length; i++)
				{
					if (this.fieldNumbers[i] == num3)
					{
						IProtoSerializer protoSerializer = this.serializers[i];
						if (value == null && protoSerializer.ExpectedType == this.forType)
						{
							value = this.CreateInstance(source);
						}
						if (protoSerializer.ReturnsValue)
						{
							value = protoSerializer.Read(value, source);
						}
						else
						{
							protoSerializer.Read(value, source);
						}
						num2 = i;
						num = num3;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (value == null)
					{
						value = this.CreateInstance(source);
					}
					if (this.isExtensible)
					{
						source.AppendExtensionData((IExtensible)value);
					}
					else
					{
						source.SkipField();
					}
				}
			}
			if (value == null)
			{
				value = this.CreateInstance(source);
			}
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.AfterDeserialize, source.Context);
			}
			return value;
		}

		private object InvokeCallback(MethodInfo method, object obj, SerializationContext context)
		{
			object result = null;
			if (method != null)
			{
				bool flag = false;
				ParameterInfo[] parameters = method.GetParameters();
				int num = parameters.Length;
				if (num != 0)
				{
					if (num == 1)
					{
						Type parameterType = parameters[0].ParameterType;
						if (parameterType == typeof(SerializationContext))
						{
							result = method.Invoke(obj, new object[]
							{
								context
							});
							flag = true;
						}
					}
				}
				else
				{
					result = method.Invoke(obj, null);
					flag = true;
				}
				if (!flag)
				{
					throw CallbackSet.CreateInvalidCallbackSignature(method);
				}
			}
			return result;
		}

		private object CreateInstance(ProtoReader source)
		{
			object obj;
			if (this.factory != null)
			{
				obj = this.InvokeCallback(this.factory, null, source.Context);
			}
			else if (this.useConstructor)
			{
				if (!this.hasConstructor)
				{
					TypeModel.ThrowCannotCreateInstance(this.constructType);
				}
				obj = Activator.CreateInstance(this.constructType, true);
			}
			else
			{
				obj = BclHelpers.GetUninitializedObject(this.constructType);
			}
			ProtoReader.NoteObject(obj, source);
			if (this.baseCtorCallbacks != null)
			{
				for (int i = 0; i < this.baseCtorCallbacks.Length; i++)
				{
					this.InvokeCallback(this.baseCtorCallbacks[i], obj, source.Context);
				}
			}
			if (this.callbacks != null)
			{
				this.InvokeCallback(this.callbacks.BeforeDeserialize, obj, source.Context);
			}
			return obj;
		}

		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return false;
			}
		}

		private readonly Type forType;

		private readonly Type constructType;

		private readonly IProtoSerializer[] serializers;

		private readonly int[] fieldNumbers;

		private readonly bool isRootType;

		private readonly bool useConstructor;

		private readonly bool isExtensible;

		private readonly bool hasConstructor;

		private readonly CallbackSet callbacks;

		private readonly MethodInfo[] baseCtorCallbacks;

		private readonly MethodInfo factory;

		private static readonly Type iextensible = typeof(IExtensible);
	}
}
