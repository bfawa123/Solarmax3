using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class SurrogateSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		public SurrogateSerializer(Type forType, Type declaredType, IProtoTypeSerializer rootTail)
		{
			this.forType = forType;
			this.declaredType = declaredType;
			this.rootTail = rootTail;
			this.toTail = this.GetConversion(true);
			this.fromTail = this.GetConversion(false);
		}

		bool IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		public bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		public bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		private static bool HasCast(Type type, Type from, Type to, out MethodInfo op)
		{
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if ((!(methodInfo.Name != "op_Implicit") || !(methodInfo.Name != "op_Explicit")) && methodInfo.ReturnType == to)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == from)
					{
						op = methodInfo;
						return true;
					}
				}
			}
			op = null;
			return false;
		}

		public MethodInfo GetConversion(bool toTail)
		{
			Type to = (!toTail) ? this.forType : this.declaredType;
			Type from = (!toTail) ? this.declaredType : this.forType;
			MethodInfo result;
			if (SurrogateSerializer.HasCast(this.declaredType, from, to, out result) || SurrogateSerializer.HasCast(this.forType, from, to, out result))
			{
				return result;
			}
			throw new InvalidOperationException("No suitable conversion operator found for surrogate: " + this.forType.FullName + " / " + this.declaredType.FullName);
		}

		public void Write(object value, ProtoWriter writer)
		{
			this.rootTail.Write(this.toTail.Invoke(null, new object[]
			{
				value
			}), writer);
		}

		public object Read(object value, ProtoReader source)
		{
			object[] array = new object[]
			{
				value
			};
			value = this.toTail.Invoke(null, array);
			array[0] = this.rootTail.Read(value, source);
			return this.fromTail.Invoke(null, array);
		}

		private readonly Type forType;

		private readonly Type declaredType;

		private readonly MethodInfo toTail;

		private readonly MethodInfo fromTail;

		private IProtoTypeSerializer rootTail;
	}
}
