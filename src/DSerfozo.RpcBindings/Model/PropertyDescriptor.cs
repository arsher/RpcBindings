﻿using System;
using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Model
{
    [DataContract]
    public class PropertyDescriptor
    {
        public sealed class Builder
        {
            private readonly PropertyDescriptor constructed = new PropertyDescriptor()
            {
                Readable = true,
                Writable = true
            };

            public Builder WithId(long id)
            {
                constructed.Id = id;
                return this;
            }

            public Builder WithName(string name)
            {
                constructed.Name = name;
                return this;
            }

            public Builder WithGetter(Func<object, object> getter)
            {
                constructed.Getter = getter;
                return this;
            }

            public Builder WithSetter(Action<object, object> setter)
            {
                constructed.Setter = setter;
                return this;
            }

            public Builder WithReadOnly(bool readOnly)
            {
                constructed.Writable = !readOnly;
                return this;
            }

            public Builder WithType(Type type)
            {
                constructed.Type = type;
                return this;
            }

            public Builder WithValue(object value)
            {
                constructed.Value = value;
                constructed.IsValueSet = true;
                return this;
            }

            public PropertyDescriptor Get()
            {
                return constructed;
            }
        }

        [DataMember]
        public long Id { get; protected set;  }

        [DataMember]
        public string Name { get; protected set; }

        [DataMember]
        public bool Readable { get; private set; }

        [DataMember]
        public bool Writable { get; private set; }

        [DataMember]
        public object Value { get; private set; }

        [DataMember]
        public bool IsValueSet { get; private set; }

        public Func<object, object> Getter { get; private set; }

        public Action<object, object> Setter { get; private set; }

        public Type Type { get; private set; }

        protected PropertyDescriptor()
        {
            
        }

        public static Builder Create()
        {
            return new Builder();
        }
    }
}
