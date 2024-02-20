using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SimpleSetSketching
{
	public static class DynamicConstantTypeCreator<T> where T : struct
	{
		static IDictionary<T, Type> _types = new Dictionary<T, Type>();
		static IDictionary<Type, T> _values = new Dictionary<Type, T>();
		static AssemblyName _assemblyName = new AssemblyName("DynamicAssembly");
		static AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndCollect);
		static ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("DynamicModule");

		public static Type GetConstant(T value)
		{
			if (!_types.ContainsKey(value)) CreateConstant($"DynamicConst-{typeof(T).Name}", value);
			return _types[value];
		}

		public static T GetValue(Type type)
		{
			return _values[type];
		}

		static void CreateConstant(string name, T constantValue)
		{


			TypeBuilder typeBuilder = _moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout, typeof(ValueType), new Type[] { typeof(IConstant<T>) });

			// Set value
			FieldBuilder fieldBuilder = typeBuilder.DefineField("value", typeof(T), FieldAttributes.Public | FieldAttributes.Literal);
			fieldBuilder.SetConstant(constantValue);

			// Add a constructor
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(T) });
			ILGenerator constructorIL = constructorBuilder.GetILGenerator();
			constructorIL.Emit(OpCodes.Ldarg_0);
			constructorIL.Emit(OpCodes.Ldarg_1);
			constructorIL.Emit(OpCodes.Stfld, fieldBuilder);
			constructorIL.Emit(OpCodes.Ret);

			// Add a method to get the constant value
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, typeof(T), Type.EmptyTypes);
			ILGenerator methodIL = methodBuilder.GetILGenerator();
			methodIL.Emit(OpCodes.Ldarg_0);
			methodIL.Emit(OpCodes.Ldfld, fieldBuilder);
			methodIL.Emit(OpCodes.Ret);

			// Add to interface
			typeBuilder.DefineMethodOverride(methodBuilder, typeof(IConstant<T>).GetMethod("Get"));

			Type dynamicType = typeBuilder.CreateType();
			_types[constantValue] = dynamicType;
			_values[dynamicType] = constantValue;
		}
	}


}

