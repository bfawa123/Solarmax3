using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ProtoBuf.Meta
{
	public abstract class TypeModel
	{
		protected internal Type MapType(Type type)
		{
			return this.MapType(type, true);
		}

		protected internal virtual Type MapType(Type type, bool demand)
		{
			return type;
		}

		private WireType GetWireType(ProtoTypeCode code, DataFormat format, ref Type type, out int modelKey)
		{
			modelKey = -1;
			if (Helpers.IsEnum(type))
			{
				modelKey = this.GetKey(ref type);
				return WireType.Variant;
			}
			switch (code)
			{
			case ProtoTypeCode.Boolean:
			case ProtoTypeCode.Char:
			case ProtoTypeCode.SByte:
			case ProtoTypeCode.Byte:
			case ProtoTypeCode.Int16:
			case ProtoTypeCode.UInt16:
			case ProtoTypeCode.Int32:
			case ProtoTypeCode.UInt32:
				return (format != DataFormat.FixedSize) ? WireType.Variant : WireType.Fixed32;
			case ProtoTypeCode.Int64:
			case ProtoTypeCode.UInt64:
				return (format != DataFormat.FixedSize) ? WireType.Variant : WireType.Fixed64;
			case ProtoTypeCode.Single:
				return WireType.Fixed32;
			case ProtoTypeCode.Double:
				return WireType.Fixed64;
			case ProtoTypeCode.Decimal:
			case ProtoTypeCode.DateTime:
			case ProtoTypeCode.String:
				break;
			default:
				switch (code)
				{
				case ProtoTypeCode.TimeSpan:
				case ProtoTypeCode.ByteArray:
				case ProtoTypeCode.Guid:
				case ProtoTypeCode.Uri:
					break;
				default:
					if ((modelKey = this.GetKey(ref type)) >= 0)
					{
						return WireType.String;
					}
					return WireType.None;
				}
				break;
			}
			return WireType.String;
		}

		internal bool TrySerializeAuxiliaryType(ProtoWriter writer, Type type, DataFormat format, int tag, object value, bool isInsideList)
		{
			if (type == null)
			{
				type = value.GetType();
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			int num;
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			if (num >= 0)
			{
				if (Helpers.IsEnum(type))
				{
					this.Serialize(num, value, writer);
					return true;
				}
				ProtoWriter.WriteFieldHeader(tag, wireType, writer);
				switch (wireType + 1)
				{
				case WireType.Variant:
					throw ProtoWriter.CreateException(writer);
				case WireType.StartGroup:
				case WireType.EndGroup:
				{
					SubItemToken token = ProtoWriter.StartSubItem(value, writer);
					this.Serialize(num, value, writer);
					ProtoWriter.EndSubItem(token, writer);
					return true;
				}
				}
				this.Serialize(num, value, writer);
				return true;
			}
			else
			{
				if (wireType != WireType.None)
				{
					ProtoWriter.WriteFieldHeader(tag, wireType, writer);
				}
				switch (typeCode)
				{
				case ProtoTypeCode.Boolean:
					ProtoWriter.WriteBoolean((bool)value, writer);
					return true;
				case ProtoTypeCode.Char:
					ProtoWriter.WriteUInt16((ushort)((char)value), writer);
					return true;
				case ProtoTypeCode.SByte:
					ProtoWriter.WriteSByte((sbyte)value, writer);
					return true;
				case ProtoTypeCode.Byte:
					ProtoWriter.WriteByte((byte)value, writer);
					return true;
				case ProtoTypeCode.Int16:
					ProtoWriter.WriteInt16((short)value, writer);
					return true;
				case ProtoTypeCode.UInt16:
					ProtoWriter.WriteUInt16((ushort)value, writer);
					return true;
				case ProtoTypeCode.Int32:
					ProtoWriter.WriteInt32((int)value, writer);
					return true;
				case ProtoTypeCode.UInt32:
					ProtoWriter.WriteUInt32((uint)value, writer);
					return true;
				case ProtoTypeCode.Int64:
					ProtoWriter.WriteInt64((long)value, writer);
					return true;
				case ProtoTypeCode.UInt64:
					ProtoWriter.WriteUInt64((ulong)value, writer);
					return true;
				case ProtoTypeCode.Single:
					ProtoWriter.WriteSingle((float)value, writer);
					return true;
				case ProtoTypeCode.Double:
					ProtoWriter.WriteDouble((double)value, writer);
					return true;
				case ProtoTypeCode.Decimal:
					BclHelpers.WriteDecimal((decimal)value, writer);
					return true;
				case ProtoTypeCode.DateTime:
					BclHelpers.WriteDateTime((DateTime)value, writer);
					return true;
				default:
					switch (typeCode)
					{
					case ProtoTypeCode.TimeSpan:
						BclHelpers.WriteTimeSpan((TimeSpan)value, writer);
						return true;
					case ProtoTypeCode.ByteArray:
						ProtoWriter.WriteBytes((byte[])value, writer);
						return true;
					case ProtoTypeCode.Guid:
						BclHelpers.WriteGuid((Guid)value, writer);
						return true;
					case ProtoTypeCode.Uri:
						ProtoWriter.WriteString(((Uri)value).AbsoluteUri, writer);
						return true;
					default:
					{
						IEnumerable enumerable = value as IEnumerable;
						if (enumerable == null)
						{
							return false;
						}
						if (isInsideList)
						{
							throw TypeModel.CreateNestedListsNotSupported();
						}
						IEnumerator enumerator = enumerable.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								if (obj == null)
								{
									throw new NullReferenceException();
								}
								if (!this.TrySerializeAuxiliaryType(writer, null, format, tag, obj, true))
								{
									TypeModel.ThrowUnexpectedType(obj.GetType());
								}
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
						return true;
					}
					}
					break;
				case ProtoTypeCode.String:
					ProtoWriter.WriteString((string)value, writer);
					return true;
				}
			}
		}

		private void SerializeCore(ProtoWriter writer, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Type type = value.GetType();
			int key = this.GetKey(ref type);
			if (key >= 0)
			{
				this.Serialize(key, value, writer);
			}
			else if (!this.TrySerializeAuxiliaryType(writer, type, DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(type);
			}
		}

		public void Serialize(Stream dest, object value)
		{
			this.Serialize(dest, value, null);
		}

		public void Serialize(Stream dest, object value, SerializationContext context)
		{
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				protoWriter.SetRootObject(value);
				this.SerializeCore(protoWriter, value);
				protoWriter.Close();
			}
		}

		public void Serialize(ProtoWriter dest, object value)
		{
			dest.CheckDepthFlushlock();
			dest.SetRootObject(value);
			this.SerializeCore(dest, value);
			dest.CheckDepthFlushlock();
			ProtoWriter.Flush(dest);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, fieldNumber, null, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead)
		{
			bool flag;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out bytesRead, out flag, null);
		}

		private object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead, out bool haveObject, SerializationContext context)
		{
			haveObject = false;
			bytesRead = 0;
			if (type == null && (style != PrefixStyle.Base128 || resolver == null))
			{
				throw new InvalidOperationException("A type must be provided unless base-128 prefixing is being used in combination with a resolver");
			}
			int num;
			for (;;)
			{
				bool flag = expectedField > 0 || resolver != null;
				int num2;
				int num3;
				num = ProtoReader.ReadLengthPrefix(source, flag, style, out num2, out num3);
				if (num3 == 0)
				{
					break;
				}
				bytesRead += num3;
				if (num < 0)
				{
					return value;
				}
				bool flag2;
				if (style != PrefixStyle.Base128)
				{
					flag2 = false;
				}
				else if (flag && expectedField == 0 && type == null && resolver != null)
				{
					type = resolver(num2);
					flag2 = (type == null);
				}
				else
				{
					flag2 = (expectedField != num2);
				}
				if (flag2)
				{
					if (num == 2147483647)
					{
						goto Block_12;
					}
					ProtoReader.Seek(source, num, null);
					bytesRead += num;
				}
				if (!flag2)
				{
					goto Block_13;
				}
			}
			return value;
			Block_12:
			throw new InvalidOperationException();
			Block_13:
			object result;
			using (ProtoReader protoReader = new ProtoReader(source, this, context, num))
			{
				int key = this.GetKey(ref type);
				if (key >= 0)
				{
					value = this.Deserialize(key, value, protoReader);
				}
				else if (!this.TryDeserializeAuxiliaryType(protoReader, DataFormat.Default, 1, type, ref value, true, false, true, false) && num != 0)
				{
					TypeModel.ThrowUnexpectedType(type);
				}
				bytesRead += protoReader.Position;
				haveObject = true;
				result = value;
			}
			return result;
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			return this.DeserializeItems(source, type, style, expectedField, resolver, null);
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator(this, source, type, style, expectedField, resolver, context);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField)
		{
			return this.DeserializeItems<T>(source, style, expectedField, null);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator<T>(this, source, style, expectedField, context);
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			this.SerializeWithLengthPrefix(dest, value, type, style, fieldNumber, null);
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber, SerializationContext context)
		{
			if (type == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				type = this.MapType(value.GetType());
			}
			int key = this.GetKey(ref type);
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				switch (style)
				{
				case PrefixStyle.None:
					this.Serialize(key, value, protoWriter);
					break;
				case PrefixStyle.Base128:
				case PrefixStyle.Fixed32:
				case PrefixStyle.Fixed32BigEndian:
					ProtoWriter.WriteObject(value, key, protoWriter, style, fieldNumber);
					break;
				default:
					throw new ArgumentOutOfRangeException("style");
				}
				protoWriter.Close();
			}
		}

		public object Deserialize(Stream source, object value, Type type)
		{
			return this.Deserialize(source, value, type, null);
		}

		public object Deserialize(Stream source, object value, Type type, SerializationContext context)
		{
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			object result;
			using (ProtoReader protoReader = new ProtoReader(source, this, context))
			{
				if (value != null)
				{
					protoReader.SetRootObject(value);
				}
				result = this.DeserializeCore(protoReader, type, value, noAutoCreate);
			}
			return result;
		}

		private bool PrepareDeserialize(object value, ref Type type)
		{
			if (type == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("type");
				}
				type = this.MapType(value.GetType());
			}
			bool result = true;
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
				result = false;
			}
			return result;
		}

		public object Deserialize(Stream source, object value, Type type, int length)
		{
			return this.Deserialize(source, value, type, length, null);
		}

		public object Deserialize(Stream source, object value, Type type, int length, SerializationContext context)
		{
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			object result;
			using (ProtoReader protoReader = new ProtoReader(source, this, context, length))
			{
				if (value != null)
				{
					protoReader.SetRootObject(value);
				}
				object obj = this.DeserializeCore(protoReader, type, value, noAutoCreate);
				protoReader.CheckFullyConsumed();
				result = obj;
			}
			return result;
		}

		public object Deserialize(ProtoReader source, object value, Type type)
		{
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			if (value != null)
			{
				source.SetRootObject(value);
			}
			object result = this.DeserializeCore(source, type, value, noAutoCreate);
			source.CheckFullyConsumed();
			return result;
		}

		private object DeserializeCore(ProtoReader reader, Type type, object value, bool noAutoCreate)
		{
			int key = this.GetKey(ref type);
			if (key >= 0 && !Helpers.IsEnum(type))
			{
				return this.Deserialize(key, value, reader);
			}
			this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, noAutoCreate, false);
			return value;
		}

		internal static MethodInfo ResolveListAdd(TypeModel model, Type listType, Type itemType, out bool isList)
		{
			isList = model.MapType(TypeModel.ilist).IsAssignableFrom(listType);
			Type[] array = new Type[]
			{
				itemType
			};
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(listType, "Add", array);
			if (instanceMethod == null)
			{
				Type type = model.MapType(typeof(ICollection<>)).MakeGenericType(array);
				if (type.IsAssignableFrom(listType))
				{
					instanceMethod = Helpers.GetInstanceMethod(type, "Add", array);
				}
			}
			if (instanceMethod == null)
			{
				array[0] = model.MapType(typeof(object));
				instanceMethod = Helpers.GetInstanceMethod(listType, "Add", array);
			}
			if (instanceMethod == null && isList)
			{
				instanceMethod = Helpers.GetInstanceMethod(model.MapType(TypeModel.ilist), "Add", array);
			}
			return instanceMethod;
		}

		internal static Type GetListItemType(TypeModel model, Type listType)
		{
			if (listType == model.MapType(typeof(string)) || listType.IsArray || !model.MapType(typeof(IEnumerable)).IsAssignableFrom(listType))
			{
				return null;
			}
			BasicList basicList = new BasicList();
			foreach (MethodInfo methodInfo in listType.GetMethods())
			{
				if (!methodInfo.IsStatic && !(methodInfo.Name != "Add"))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1 && !basicList.Contains(parameters[0].ParameterType))
					{
						basicList.Add(parameters[0].ParameterType);
					}
				}
			}
			foreach (Type type in listType.GetInterfaces())
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == model.MapType(typeof(ICollection<>)))
				{
					Type[] genericArguments = type.GetGenericArguments();
					if (!basicList.Contains(genericArguments[0]))
					{
						basicList.Add(genericArguments[0]);
					}
				}
			}
			foreach (PropertyInfo propertyInfo in listType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!(propertyInfo.Name != "Item") && !basicList.Contains(propertyInfo.PropertyType))
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1 && indexParameters[0].ParameterType == model.MapType(typeof(int)))
					{
						basicList.Add(propertyInfo.PropertyType);
					}
				}
			}
			int count = basicList.Count;
			if (count == 0)
			{
				return null;
			}
			if (count != 1)
			{
				if (count == 2)
				{
					if (TypeModel.CheckDictionaryAccessors(model, (Type)basicList[0], (Type)basicList[1]))
					{
						return (Type)basicList[0];
					}
					if (TypeModel.CheckDictionaryAccessors(model, (Type)basicList[1], (Type)basicList[0]))
					{
						return (Type)basicList[1];
					}
				}
				return null;
			}
			return (Type)basicList[0];
		}

		private static bool CheckDictionaryAccessors(TypeModel model, Type pair, Type value)
		{
			return pair.IsGenericType && pair.GetGenericTypeDefinition() == model.MapType(typeof(KeyValuePair<, >)) && pair.GetGenericArguments()[1] == value;
		}

		private bool TryDeserializeList(TypeModel model, ProtoReader reader, DataFormat format, int tag, Type listType, Type itemType, ref object value)
		{
			bool flag;
			MethodInfo methodInfo = TypeModel.ResolveListAdd(model, listType, itemType, out flag);
			if (methodInfo == null)
			{
				throw new NotSupportedException("Unknown list variant: " + listType.FullName);
			}
			bool result = false;
			object obj = null;
			IList list = value as IList;
			object[] array = (!flag) ? new object[1] : null;
			BasicList basicList = (!listType.IsArray) ? null : new BasicList();
			while (this.TryDeserializeAuxiliaryType(reader, format, tag, itemType, ref obj, true, true, true, true))
			{
				result = true;
				if (value == null && basicList == null)
				{
					value = TypeModel.CreateListInstance(listType, itemType);
					list = (value as IList);
				}
				if (list != null)
				{
					list.Add(obj);
				}
				else if (basicList != null)
				{
					basicList.Add(obj);
				}
				else
				{
					array[0] = obj;
					methodInfo.Invoke(value, array);
				}
				obj = null;
			}
			if (basicList != null)
			{
				if (value != null)
				{
					if (basicList.Count != 0)
					{
						Array array2 = (Array)value;
						Array array3 = Array.CreateInstance(itemType, array2.Length + basicList.Count);
						Array.Copy(array2, array3, array2.Length);
						basicList.CopyTo(array3, array2.Length);
						value = array3;
					}
				}
				else
				{
					Array array3 = Array.CreateInstance(itemType, basicList.Count);
					basicList.CopyTo(array3, 0);
					value = array3;
				}
			}
			return result;
		}

		private static object CreateListInstance(Type listType, Type itemType)
		{
			Type type = listType;
			if (listType.IsArray)
			{
				return Array.CreateInstance(itemType, 0);
			}
			if (!listType.IsClass || listType.IsAbstract || Helpers.GetConstructor(listType, Helpers.EmptyTypes, true) == null)
			{
				bool flag = false;
				if (listType.IsInterface && listType.FullName.IndexOf("Dictionary") >= 0)
				{
					if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(IDictionary<, >))
					{
						Type[] genericArguments = listType.GetGenericArguments();
						type = typeof(Dictionary<, >).MakeGenericType(genericArguments);
						flag = true;
					}
					if (!flag && listType == typeof(IDictionary))
					{
						type = typeof(Hashtable);
						flag = true;
					}
				}
				if (!flag)
				{
					type = typeof(List<>).MakeGenericType(new Type[]
					{
						itemType
					});
					flag = true;
				}
				if (!flag)
				{
					type = typeof(ArrayList);
				}
			}
			return Activator.CreateInstance(type);
		}

		internal bool TryDeserializeAuxiliaryType(ProtoReader reader, DataFormat format, int tag, Type type, ref object value, bool skipOtherFields, bool asListItem, bool autoCreate, bool insideList)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			int num;
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			bool flag = false;
			if (wireType == WireType.None)
			{
				Type type2 = TypeModel.GetListItemType(this, type);
				if (type2 == null && type.IsArray && type.GetArrayRank() == 1 && type != typeof(byte[]))
				{
					type2 = type.GetElementType();
				}
				if (type2 != null)
				{
					if (insideList)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
					flag = this.TryDeserializeList(this, reader, format, tag, type, type2, ref value);
					if (!flag && autoCreate)
					{
						value = TypeModel.CreateListInstance(type, type2);
					}
					return flag;
				}
				else
				{
					TypeModel.ThrowUnexpectedType(type);
				}
			}
			while (!flag || !asListItem)
			{
				int num2 = reader.ReadFieldHeader();
				if (num2 <= 0)
				{
					//IL_363:
					if (!flag && !asListItem && autoCreate && type != typeof(string))
					{
						value = Activator.CreateInstance(type);
					}
					return flag;
				}
				if (num2 != tag)
				{
					if (!skipOtherFields)
					{
						throw ProtoReader.AddErrorData(new InvalidOperationException(string.Concat(new object[]
						{
							"Expected field ",
							tag,
							", but found ",
							num2
						})), reader);
					}
					reader.SkipField();
				}
				else
				{
					flag = true;
					reader.Hint(wireType);
					if (num >= 0)
					{
						if (wireType != WireType.String && wireType != WireType.StartGroup)
						{
							value = this.Deserialize(num, value, reader);
						}
						else
						{
							SubItemToken token = ProtoReader.StartSubItem(reader);
							value = this.Deserialize(num, value, reader);
							ProtoReader.EndSubItem(token, reader);
						}
					}
					else
					{
						switch (typeCode)
						{
						case ProtoTypeCode.Boolean:
							value = reader.ReadBoolean();
							break;
						case ProtoTypeCode.Char:
							value = (char)reader.ReadUInt16();
							break;
						case ProtoTypeCode.SByte:
							value = reader.ReadSByte();
							break;
						case ProtoTypeCode.Byte:
							value = reader.ReadByte();
							break;
						case ProtoTypeCode.Int16:
							value = reader.ReadInt16();
							break;
						case ProtoTypeCode.UInt16:
							value = reader.ReadUInt16();
							break;
						case ProtoTypeCode.Int32:
							value = reader.ReadInt32();
							break;
						case ProtoTypeCode.UInt32:
							value = reader.ReadUInt32();
							break;
						case ProtoTypeCode.Int64:
							value = reader.ReadInt64();
							break;
						case ProtoTypeCode.UInt64:
							value = reader.ReadUInt64();
							break;
						case ProtoTypeCode.Single:
							value = reader.ReadSingle();
							break;
						case ProtoTypeCode.Double:
							value = reader.ReadDouble();
							break;
						case ProtoTypeCode.Decimal:
							value = BclHelpers.ReadDecimal(reader);
							break;
						case ProtoTypeCode.DateTime:
							value = BclHelpers.ReadDateTime(reader);
							break;
						default:
							switch (typeCode)
							{
							case ProtoTypeCode.TimeSpan:
								value = BclHelpers.ReadTimeSpan(reader);
								break;
							case ProtoTypeCode.ByteArray:
								value = ProtoReader.AppendBytes((byte[])value, reader);
								break;
							case ProtoTypeCode.Guid:
								value = BclHelpers.ReadGuid(reader);
								break;
							case ProtoTypeCode.Uri:
								value = new Uri(reader.ReadString());
								break;
							}
							break;
						case ProtoTypeCode.String:
							value = reader.ReadString();
							break;
						}
					}
				}
			}
            //goto IL_363;
            // 修复
            if (!flag && !asListItem && autoCreate && type != typeof(string))
            {
                value = Activator.CreateInstance(type);
            }
            return flag;
        }

		public static RuntimeTypeModel Create()
		{
			return new RuntimeTypeModel(false);
		}

		protected internal static Type ResolveProxies(Type type)
		{
			if (type == null)
			{
				return null;
			}
			if (type.IsGenericParameter)
			{
				return null;
			}
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				return underlyingType;
			}
			if (type.FullName.StartsWith("System.Data.Entity.DynamicProxies."))
			{
				return type.BaseType;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				string fullName = interfaces[i].FullName;
				if (fullName != null)
				{
					if (fullName == "NHibernate.Proxy.INHibernateProxy" || fullName == "NHibernate.Proxy.DynamicProxy.IProxy" || fullName == "NHibernate.Intercept.IFieldInterceptorAccessor")
					{
						return type.BaseType;
					}
				}
			}
			return null;
		}

		public bool IsDefined(Type type)
		{
			return this.GetKey(ref type) >= 0;
		}

		protected internal int GetKey(ref Type type)
		{
			int keyImpl = this.GetKeyImpl(type);
			if (keyImpl < 0)
			{
				Type type2 = TypeModel.ResolveProxies(type);
				if (type2 != null)
				{
					type = type2;
					keyImpl = this.GetKeyImpl(type);
				}
			}
			return keyImpl;
		}

		protected abstract int GetKeyImpl(Type type);

		protected internal abstract void Serialize(int key, object value, ProtoWriter dest);

		protected internal abstract object Deserialize(int key, object value, ProtoReader source);

		public object DeepClone(object value)
		{
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			int key = this.GetKey(ref type);
			if (key >= 0 && !Helpers.IsEnum(type))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ProtoWriter protoWriter = new ProtoWriter(memoryStream, this, null))
					{
						protoWriter.SetRootObject(value);
						this.Serialize(key, value, protoWriter);
						protoWriter.Close();
					}
					memoryStream.Position = 0L;
					using (ProtoReader protoReader = new ProtoReader(memoryStream, this, null))
					{
						return this.Deserialize(key, null, protoReader);
					}
				}
			}
			if (type == typeof(byte[]))
			{
				byte[] array = (byte[])value;
				byte[] array2 = new byte[array.Length];
				Helpers.BlockCopy(array, 0, array2, 0, array.Length);
				return array2;
			}
			int num;
			if (this.GetWireType(Helpers.GetTypeCode(type), DataFormat.Default, ref type, out num) != WireType.None && num < 0)
			{
				return value;
			}
			object result;
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (ProtoWriter protoWriter2 = new ProtoWriter(memoryStream2, this, null))
				{
					if (!this.TrySerializeAuxiliaryType(protoWriter2, type, DataFormat.Default, 1, value, false))
					{
						TypeModel.ThrowUnexpectedType(type);
					}
					protoWriter2.Close();
				}
				memoryStream2.Position = 0L;
				using (ProtoReader protoReader2 = new ProtoReader(memoryStream2, this, null))
				{
					value = null;
					this.TryDeserializeAuxiliaryType(protoReader2, DataFormat.Default, 1, type, ref value, true, false, true, false);
					result = value;
				}
			}
			return result;
		}

		protected internal static void ThrowUnexpectedSubtype(Type expected, Type actual)
		{
			if (expected != TypeModel.ResolveProxies(actual))
			{
				throw new InvalidOperationException("Unexpected sub-type: " + actual.FullName);
			}
		}

		protected internal static void ThrowUnexpectedType(Type type)
		{
			string str = (type != null) ? type.FullName : "(unknown)";
			if (type != null)
			{
				Type baseType = type.BaseType;
				if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name == "GeneratedMessage`2")
				{
					throw new InvalidOperationException("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: " + str);
				}
			}
			throw new InvalidOperationException("Type is not expected, and no contract can be inferred: " + str);
		}

		internal static Exception CreateNestedListsNotSupported()
		{
			return new NotSupportedException("Nested or jagged lists and arrays are not supported");
		}

		public static void ThrowCannotCreateInstance(Type type)
		{
			throw new ProtoException("No parameterless constructor found for " + type.Name);
		}

		internal static string SerializeType(TypeModel model, Type type)
		{
			TypeFormatEventHandler dynamicTypeFormatting;
			if (model != null && (dynamicTypeFormatting = model.DynamicTypeFormatting) != null)
			{
				TypeFormatEventArgs typeFormatEventArgs = new TypeFormatEventArgs(type);
				dynamicTypeFormatting(model, typeFormatEventArgs);
				if (!Helpers.IsNullOrEmpty(typeFormatEventArgs.FormattedName))
				{
					return typeFormatEventArgs.FormattedName;
				}
			}
			return type.AssemblyQualifiedName;
		}

		internal static Type DeserializeType(TypeModel model, string value)
		{
			TypeFormatEventHandler dynamicTypeFormatting;
			if (model != null && (dynamicTypeFormatting = model.DynamicTypeFormatting) != null)
			{
				TypeFormatEventArgs typeFormatEventArgs = new TypeFormatEventArgs(value);
				dynamicTypeFormatting(model, typeFormatEventArgs);
				if (typeFormatEventArgs.Type != null)
				{
					return typeFormatEventArgs.Type;
				}
			}
			return Type.GetType(value);
		}

		public bool CanSerializeContractType(Type type)
		{
			return this.CanSerialize(type, false, true, true);
		}

		public bool CanSerialize(Type type)
		{
			return this.CanSerialize(type, true, true, true);
		}

		public bool CanSerializeBasicType(Type type)
		{
			return this.CanSerialize(type, true, false, true);
		}

		private bool CanSerialize(Type type, bool allowBasic, bool allowContract, bool allowLists)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			if (typeCode != ProtoTypeCode.Empty && typeCode != ProtoTypeCode.Unknown)
			{
				return allowBasic;
			}
			int key = this.GetKey(ref type);
			if (key >= 0)
			{
				return allowContract;
			}
			if (allowLists)
			{
				Type type2 = null;
				if (type.IsArray)
				{
					if (type.GetArrayRank() == 1)
					{
						type2 = type.GetElementType();
					}
				}
				else
				{
					type2 = TypeModel.GetListItemType(this, type);
				}
				if (type2 != null)
				{
					return this.CanSerialize(type2, allowBasic, allowContract, false);
				}
			}
			return false;
		}

		public virtual string GetSchema(Type type)
		{
			throw new NotSupportedException();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		// 修复
		public event TypeFormatEventHandler DynamicTypeFormatting;

		internal virtual Type GetType(string fullName, Assembly context)
		{
			return TypeModel.ResolveKnownType(fullName, this, context);
		}

		internal static Type ResolveKnownType(string name, TypeModel model, Assembly assembly)
		{
			if (Helpers.IsNullOrEmpty(name))
			{
				return null;
			}
			try
			{
				Type type = Type.GetType(name);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
			try
			{
				int num = name.IndexOf(',');
				string name2 = ((num <= 0) ? name : name.Substring(0, num)).Trim();
				if (assembly == null)
				{
					assembly = Assembly.GetCallingAssembly();
				}
				Type type2 = (assembly != null) ? assembly.GetType(name2) : null;
				if (type2 != null)
				{
					return type2;
				}
			}
			catch
			{
			}
			return null;
		}

		private static readonly Type ilist = typeof(IList);

		private class DeserializeItemsIterator<T> : TypeModel.DeserializeItemsIterator, IEnumerator<T>, IEnumerable<T>, IEnumerator, IDisposable, IEnumerable
		{
			public DeserializeItemsIterator(TypeModel model, Stream source, PrefixStyle style, int expectedField, SerializationContext context) : base(model, source, model.MapType(typeof(T)), style, expectedField, null, context)
			{
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this;
			}

			public new T Current
			{
				get
				{
					return (T)((object)base.Current);
				}
			}

			void IDisposable.Dispose()
			{
			}
		}

		private class DeserializeItemsIterator : IEnumerator, IEnumerable
		{
			public DeserializeItemsIterator(TypeModel model, Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
			{
				this.haveObject = true;
				this.source = source;
				this.type = type;
				this.style = style;
				this.expectedField = expectedField;
				this.resolver = resolver;
				this.model = model;
				this.context = context;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			public bool MoveNext()
			{
				if (this.haveObject)
				{
					int num;
					this.current = this.model.DeserializeWithLengthPrefix(this.source, null, this.type, this.style, this.expectedField, this.resolver, out num, out this.haveObject, this.context);
				}
				return this.haveObject;
			}

			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			public object Current
			{
				get
				{
					return this.current;
				}
			}

			private bool haveObject;

			private object current;

			private readonly Stream source;

			private readonly Type type;

			private readonly PrefixStyle style;

			private readonly int expectedField;

			private readonly Serializer.TypeResolver resolver;

			private readonly TypeModel model;

			private readonly SerializationContext context;
		}

		protected internal enum CallbackType
		{
			BeforeSerialize,
			AfterSerialize,
			BeforeDeserialize,
			AfterDeserialize
		}
	}
}
