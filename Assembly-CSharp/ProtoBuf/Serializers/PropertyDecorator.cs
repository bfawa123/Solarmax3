using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class PropertyDecorator : ProtoDecoratorBase
	{
		public PropertyDecorator(TypeModel model, Type forType, PropertyInfo property, IProtoSerializer tail) : base(tail)
		{
			this.forType = forType;
			this.property = property;
			PropertyDecorator.SanityCheck(model, property, tail, out this.readOptionsWriteValue, true);
			this.shadowSetter = PropertyDecorator.GetShadowSetter(model, property);
		}

		public override Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		private static void SanityCheck(TypeModel model, PropertyInfo property, IProtoSerializer tail, out bool writeValue, bool nonPublic)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			writeValue = (tail.ReturnsValue && (PropertyDecorator.GetShadowSetter(model, property) != null || (property.CanWrite && Helpers.GetSetMethod(property, nonPublic) != null)));
			if (!property.CanRead || Helpers.GetGetMethod(property, nonPublic) == null)
			{
				throw new InvalidOperationException("Cannot serialize property without a get accessor");
			}
			if (!writeValue && (!tail.RequiresOldValue || Helpers.IsValueType(tail.ExpectedType)))
			{
				throw new InvalidOperationException("Cannot apply changes to property " + property.DeclaringType.FullName + "." + property.Name);
			}
		}

		private static MethodInfo GetShadowSetter(TypeModel model, PropertyInfo property)
		{
			Type reflectedType = property.ReflectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(reflectedType, "Set" + property.Name, new Type[]
			{
				property.PropertyType
			});
			if (instanceMethod == null || !instanceMethod.IsPublic || instanceMethod.ReturnType != model.MapType(typeof(void)))
			{
				return null;
			}
			return instanceMethod;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			value = this.property.GetValue(value, null);
			if (value != null)
			{
				this.Tail.Write(value, dest);
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			object value2 = (!this.Tail.RequiresOldValue) ? null : this.property.GetValue(value, null);
			object obj = this.Tail.Read(value2, source);
			if (this.readOptionsWriteValue && obj != null)
			{
				if (this.shadowSetter == null)
				{
					this.property.SetValue(value, obj, null);
				}
				else
				{
					this.shadowSetter.Invoke(value, new object[]
					{
						obj
					});
				}
			}
			return null;
		}

		internal static bool CanWrite(TypeModel model, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.CanWrite || PropertyDecorator.GetShadowSetter(model, propertyInfo) != null;
			}
			return member is FieldInfo;
		}

		private readonly PropertyInfo property;

		private readonly Type forType;

		private readonly bool readOptionsWriteValue;

		private readonly MethodInfo shadowSetter;
	}
}
