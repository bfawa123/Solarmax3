using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class TupleSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		public TupleSerializer(RuntimeTypeModel model, ConstructorInfo ctor, MemberInfo[] members)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			this.ctor = ctor;
			this.members = members;
			this.tails = new IProtoSerializer[members.Length];
			ParameterInfo[] parameters = ctor.GetParameters();
			for (int i = 0; i < members.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				Type type = null;
				Type concreteType = null;
				MetaType.ResolveListTypes(model, parameterType, ref type, ref concreteType);
				Type type2 = (type != null) ? type : parameterType;
				WireType wireType;
				IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(model, DataFormat.Default, type2, out wireType, false, false, false, true);
				if (protoSerializer == null)
				{
					throw new InvalidOperationException("No serializer defined for type: " + type2.FullName);
				}
				protoSerializer = new TagDecorator(i + 1, wireType, false, protoSerializer);
				IProtoSerializer protoSerializer2;
				if (type == null)
				{
					protoSerializer2 = protoSerializer;
				}
				else if (parameterType.IsArray)
				{
					protoSerializer2 = new ArrayDecorator(model, protoSerializer, i + 1, false, wireType, parameterType, false, false);
				}
				else
				{
					protoSerializer2 = new ListDecorator(model, parameterType, concreteType, protoSerializer, i + 1, false, wireType, true, false, false);
				}
				this.tails[i] = protoSerializer2;
			}
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return this.ctor.DeclaringType;
			}
		}

		private object GetValue(object obj, int index)
		{
			PropertyInfo propertyInfo;
			if ((propertyInfo = (this.members[index] as PropertyInfo)) != null)
			{
				if (obj == null)
				{
					return (!Helpers.IsValueType(propertyInfo.PropertyType)) ? null : Activator.CreateInstance(propertyInfo.PropertyType);
				}
				return propertyInfo.GetValue(obj, null);
			}
			else
			{
				FieldInfo fieldInfo;
				if ((fieldInfo = (this.members[index] as FieldInfo)) == null)
				{
					throw new InvalidOperationException();
				}
				if (obj == null)
				{
					return (!Helpers.IsValueType(fieldInfo.FieldType)) ? null : Activator.CreateInstance(fieldInfo.FieldType);
				}
				return fieldInfo.GetValue(obj);
			}
		}

		public object Read(object value, ProtoReader source)
		{
			object[] array = new object[this.members.Length];
			bool flag = false;
			if (value == null)
			{
				flag = true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.GetValue(value, i);
			}
			int num;
			while ((num = source.ReadFieldHeader()) > 0)
			{
				flag = true;
				if (num <= this.tails.Length)
				{
					IProtoSerializer protoSerializer = this.tails[num - 1];
					array[num - 1] = this.tails[num - 1].Read((!protoSerializer.RequiresOldValue) ? null : array[num - 1], source);
				}
				else
				{
					source.SkipField();
				}
			}
			return (!flag) ? value : this.ctor.Invoke(array);
		}

		public void Write(object value, ProtoWriter dest)
		{
			for (int i = 0; i < this.tails.Length; i++)
			{
				object value2 = this.GetValue(value, i);
				if (value2 != null)
				{
					this.tails[i].Write(value2, dest);
				}
			}
		}

		public bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		private Type GetMemberType(int index)
		{
			Type memberType = Helpers.GetMemberType(this.members[index]);
			if (memberType == null)
			{
				throw new InvalidOperationException();
			}
			return memberType;
		}

		private readonly MemberInfo[] members;

		private readonly ConstructorInfo ctor;

		private IProtoSerializer[] tails;
	}
}
