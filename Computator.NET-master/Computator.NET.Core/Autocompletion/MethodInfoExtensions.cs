using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Computator.NET.Core.Autocompletion
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        ///     Return the method signature as a string.
        /// </summary>
        /// <param name="property">
        ///     The property to act on.
        /// </param>
        /// <returns>
        ///     Method signature.
        /// </returns>
        public static string GetSignature(this PropertyInfo property)
        {
            var getter = property.GetGetMethod();
            var setter = property.GetSetMethod();

            var sigBuilder = new StringBuilder();
            var primaryDef = LeastRestrictiveVisibility(getter, setter);


            BuildReturnSignature(sigBuilder, primaryDef);
            sigBuilder.Append(" { ");
            if (getter != null)
            {
                if (primaryDef != getter)
                {
                    sigBuilder.Append(Visibility(getter) + " ");
                }
                sigBuilder.Append("get; ");
            }
            if (setter != null)
            {
                if (primaryDef != setter)
                {
                    sigBuilder.Append(Visibility(setter) + " ");
                }
                sigBuilder.Append("set; ");
            }
            sigBuilder.Append("}");
            return sigBuilder.ToString();
        }

        /// <summary>
        ///     Return the method signature as a string.
        /// </summary>
        /// <param name="method">
        ///     The Method.
        /// </param>
        /// <param name="callable">
        ///     Return as an callable string(public void a(string b) would return a(b))
        /// </param>
        /// <returns>
        ///     Method signature.
        /// </returns>
        public static string GetSignature(this MethodInfo method, bool callable = false)
        {
            var sigBuilder = new StringBuilder();

            BuildReturnSignature(sigBuilder, method, callable);

            sigBuilder.Append("(");
            var firstParam = true;
            var secondParam = false;

            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefined(typeof(ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParam = true;
                            continue;
                        }
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam)
                    secondParam = false;
                else
                    sigBuilder.Append(", ");
                if (param.IsOut)
                    sigBuilder.Append("out ");
                else if (param.ParameterType.IsByRef)
                    sigBuilder.Append("ref ");

                if (IsParamArray(param))
                {
                    sigBuilder.Append("params ");
                }

                if (!callable)
                {
                    sigBuilder.Append(TypeName(param.ParameterType));
                    sigBuilder.Append(' ');
                }


                sigBuilder.Append(param.Name);

                if (param.IsOptional)
                {
                    sigBuilder.Append(" = " +
                                      (param.DefaultValue ?? "null")
                        );
                }
            }
            sigBuilder.Append(")");

            // generic constraints


            foreach (var arg in method.GetGenericArguments())
            {
                var constraints = new List<string>();
                foreach (var constraint in arg.GetGenericParameterConstraints())
                {
                    constraints.Add(TypeName(constraint));
                }

                var attrs = arg.GenericParameterAttributes;

                if (attrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                {
                    constraints.Add("class");
                }
                if (attrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                {
                    constraints.Add("struct");
                }
                if (attrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                {
                    constraints.Add("new()");
                }
                if (constraints.Count > 0)
                {
                    sigBuilder.Append(" where " + TypeName(arg) + ": " + string.Join(", ", constraints));
                }
            }


            return sigBuilder.ToString();
        }

        /// <summary>
        ///     Get full type name with full namespace names.
        /// </summary>
        /// <param name="type">
        ///     Type. May be generic or nullable.
        /// </param>
        /// <returns>
        ///     Full type name, fully qualified namespaces.
        /// </returns>
        public static string TypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                return TypeName(nullableType) + "?";
            }


            if (!type.IsGenericType)
            {
                if (type.IsArray)
                {
                    return TypeName(type.GetElementType()) + "[]";
                }

                //if (type.Si
                return TypeNameToAliasTsl(type.Name);
            }

            var sb = new StringBuilder(type.Name.Substring(0,
                type.Name.IndexOf('`'))
                );

            sb.Append('<');
            var first = true;
            foreach (var t in type.GetGenericArguments())
            {
                if (!first)
                    sb.Append(',');
                sb.Append(TypeName(t));
                first = false;
            }
            sb.Append('>');
            return sb.ToString();
        }


        private static string TypeNameToAliasCs(Type type)
        {
            switch (type.Name)
            {
                case "String":
                    return "string";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Object":
                    return "object";
                case "Void":
                    return "void";

                default:
                {
                    return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
                }
            }
        }

        private static string TypeNameToAliasTsl(string typeName)
        {
            switch (typeName)
            {
                case "Boolean":
                    return "bool";

                case "Double":
                    return "real";

                case "Int32":
                case "Int64":
                case "Int16":
                    return "integer";
                case "Uint32":
                case "Uint16":
                case "Uint64":
                    return "natural";

                case "Matrix":
                case "Matrix`1":
                case "matrix":
                case "DenseMatrix":
                case "SparseMatrix":
                    return "Matrix";

                case "Void":
                case "String":
                case "Complex":
                    return typeName.ToLower();


                default:
                    return typeName;
            }
        }

        private static void BuildReturnSignature(StringBuilder sigBuilder, MethodInfo method, bool callable = false)
        {
            var firstParam = true;
            if (callable == false)
            {
                sigBuilder.Append(Visibility(method) + " ");

                if (method.IsStatic)
                    sigBuilder.Append("static ");
                sigBuilder.Append(TypeName(method.ReturnType));
                sigBuilder.Append(' ');
            }
            sigBuilder.Append(method.Name);

            // Add method generics
            if (method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    if (firstParam)
                        firstParam = false;
                    else
                        sigBuilder.Append(", ");
                    sigBuilder.Append(TypeName(g));
                }
                sigBuilder.Append(">");
            }
        }

        private static string Visibility(MethodInfo method)
        {
            if (method.IsPublic)
                return "public";
            if (method.IsPrivate)
                return "private";
            if (method.IsAssembly)
                return "internal";
            if (method.IsFamily)
                return "protected";
            throw new Exception("I wasn't able to parse the visibility of this method.");
        }

        private static MethodInfo LeastRestrictiveVisibility(MethodInfo member1, MethodInfo member2)
        {
            if (member1 != null && member2 == null)
            {
                return member1;
            }
            if (member2 != null && member1 == null)
            {
                return member2;
            }

            var vis1 = VisibilityValue(member1);
            var vis2 = VisibilityValue(member2);
            if (vis1 < vis2)
            {
                return member1;
            }
            return member2;
        }

        private static int VisibilityValue(MethodInfo method)
        {
            if (method.IsPublic)
                return 1;
            if (method.IsFamily)
                return 2;
            if (method.IsAssembly)
                return 3;
            if (method.IsPrivate)
                return 4;
            throw new Exception("I wasn't able to parse the visibility of this method.");
        }

        public static bool IsParamArray(ParameterInfo info)
        {
            return info.GetCustomAttributes(typeof(ParamArrayAttribute), true) != null &&
                   info.GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0;
        }
    }
}