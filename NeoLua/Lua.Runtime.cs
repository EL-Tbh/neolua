﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neo.IronLua
{
	#region -- enum LuaRuntimeHelper ----------------------------------------------------

	///////////////////////////////////////////////////////////////////////////////
	/// <summary>Enumeration with the runtime-functions.</summary>
	internal enum LuaRuntimeHelper
	{
		/// <summary>Sets the table from an initializion list.</summary>
		TableSetObjects,
		/// <summary>Concats Result-Array</summary>
		ConcatArrays
	} // enum LuaRuntimeHelper

	#endregion

	#region -- class Lua ----------------------------------------------------------------

	///////////////////////////////////////////////////////////////////////////////
	/// <summary>All static methods for the language implementation</summary>
	public partial class Lua
	{
		internal const ExpressionType IntegerDivide = (ExpressionType)(-100);

		// LuaResult
		internal readonly static ConstructorInfo ResultConstructorInfoArg1;
		internal readonly static ConstructorInfo ResultConstructorInfoArgN;
		internal readonly static PropertyInfo ResultIndexPropertyInfo;
		internal readonly static PropertyInfo ResultValuesPropertyInfo;
		internal readonly static PropertyInfo ResultEmptyPropertyInfo;
		// LuaException
		internal readonly static ConstructorInfo RuntimeExceptionConstructorInfo;
		// LuaTable
		internal readonly static MethodInfo TableSetValueKeyStringMethodInfo;
		internal readonly static MethodInfo TableGetValueKeyStringMethodInfo;
		internal readonly static MethodInfo TableSetValueKeyIntMethodInfo;
		internal readonly static MethodInfo TableGetValueKeyIntMethodInfo;
		internal readonly static MethodInfo TableSetValueKeyObjectMethodInfo;
		internal readonly static MethodInfo TableGetValueKeyObjectMethodInfo;
		internal readonly static MethodInfo TableSetValueKeyListMethodInfo;
		internal readonly static MethodInfo TableGetValueKeyListMethodInfo;

		internal readonly static MethodInfo TableGetCallMemberMethodInfo;
		internal readonly static MethodInfo TableSetObjectMemberMethodInfo;
		internal readonly static MethodInfo TableDefineMethodLightMethodInfo;

		internal readonly static FieldInfo TableEntriesFieldInfo;
		internal readonly static MethodInfo TablePropertyChangedMethodInfo;
		internal readonly static FieldInfo TableEntryValueFieldInfo;

		internal readonly static MethodInfo TableAddMethodInfo;
		internal readonly static MethodInfo TableSubMethodInfo;
		internal readonly static MethodInfo TableMulMethodInfo;
		internal readonly static MethodInfo TableDivMethodInfo;
		internal readonly static MethodInfo TableModMethodInfo;
		internal readonly static MethodInfo TablePowMethodInfo;
		internal readonly static MethodInfo TableUnMinusMethodInfo;
		internal readonly static MethodInfo TableIDivMethodInfo;
		internal readonly static MethodInfo TableBAndMethodInfo;
		internal readonly static MethodInfo TableBOrMethodInfo;
		internal readonly static MethodInfo TableBXOrMethodInfo;
		internal readonly static MethodInfo TableBNotMethodInfo;
		internal readonly static MethodInfo TableShlMethodInfo;
		internal readonly static MethodInfo TableShrMethodInfo;
		internal readonly static MethodInfo TableConcatMethodInfo;
		internal readonly static MethodInfo TableLenMethodInfo;
		internal readonly static MethodInfo TableEqualMethodInfo;
		internal readonly static MethodInfo TableLessThanMethodInfo;
		internal readonly static MethodInfo TableLessEqualMethodInfo;
		internal readonly static MethodInfo TableIndexMethodInfo;
		internal readonly static MethodInfo TableNewIndexMethodInfo;
		internal readonly static MethodInfo TableCallMethodInfo;
		// LuaType
		internal readonly static PropertyInfo TypeClrPropertyInfo;
		internal readonly static MethodInfo TypeGetTypeMethodInfoArgIndex;
		internal readonly static MethodInfo TypeGetGenericItemMethodInfo;
		internal readonly static MethodInfo TypeGetTypeMethodInfoArgType;
		internal readonly static PropertyInfo TypeTypePropertyInfo;
		// LuaMethod
		internal readonly static ConstructorInfo MethodConstructorInfo;
		internal readonly static ConstructorInfo OverloadedMethodConstructorInfo;
		internal readonly static MethodInfo OverloadedMethodGetMethodMethodInfo;
		internal readonly static PropertyInfo MethodMethodPropertyInfo;
		internal readonly static PropertyInfo MethodNamePropertyInfo;
		internal readonly static PropertyInfo MethodInstancePropertyInfo;
		internal readonly static PropertyInfo MethodTypePropertyInfo;
		// LuaEvent
		internal readonly static ConstructorInfo EventConstructorInfo;
		internal readonly static PropertyInfo AddMethodInfoPropertyInfo;
		internal readonly static PropertyInfo RemoveMethodInfoPropertyInfo;
		internal readonly static PropertyInfo RaiseMethodInfoPropertyInfo;
		// Lua
		internal readonly static MethodInfo ParseNumberMethodInfo;
		internal readonly static MethodInfo RuntimeLengthMethodInfo;
		internal readonly static MethodInfo ConvertValueMethodInfo;
		internal readonly static MethodInfo GetResultValuesMethodInfo;
		internal readonly static MethodInfo CombineArrayWithResultMethodInfo;
		internal readonly static MethodInfo ConvertArrayMethodInfo;
		internal readonly static MethodInfo TableSetObjectsMethod;
		internal readonly static MethodInfo ConcatStringMethodInfo;
		internal readonly static MethodInfo ConvertDelegateMethodInfo;
		internal readonly static MethodInfo InitArray1MethodInfo;
		internal readonly static MethodInfo InitArrayNMethodInfo;
		// Object
		internal readonly static MethodInfo ObjectEqualsMethodInfo;
		internal readonly static MethodInfo ObjectReferenceEqualsMethodInfo;
		// Convert
		internal readonly static MethodInfo ConvertToStringMethodInfo;
		// String
		internal readonly static FieldInfo StringEmptyFieldInfo;
		internal readonly static MethodInfo StringConcatMethodInfo;
		internal readonly static PropertyInfo StringItemPropertyInfo;
		// CultureInvariant
		internal readonly static PropertyInfo CultureInvariantPropertyInfo;
		// Delegate
		internal readonly static PropertyInfo DelegateMethodPropertyInfo;
		internal readonly static MethodInfo CreateDelegateMethodInfo;
		// List
		internal readonly static PropertyInfo ListItemPropertyInfo;
		internal readonly static PropertyInfo ListCountPropertyInfo;

		#region -- sctor ------------------------------------------------------------------

		static Lua()
		{
			// LuaResult
			ResultConstructorInfoArg1 = typeof(LuaResult).GetConstructor(new Type[] { typeof(object) });
			ResultConstructorInfoArgN = typeof(LuaResult).GetConstructor(new Type[] { typeof(object[]) });
			ResultIndexPropertyInfo = typeof(LuaResult).GetProperty("Item", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
			ResultEmptyPropertyInfo = typeof(LuaResult).GetProperty("Empty", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);
			ResultValuesPropertyInfo = typeof(LuaResult).GetProperty("Values", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
#if DEBUG
			if (ResultConstructorInfoArg1 == null || ResultConstructorInfoArgN == null || ResultIndexPropertyInfo == null || ResultEmptyPropertyInfo == null || ResultValuesPropertyInfo == null)
				throw new ArgumentNullException("@1", "LuaResult");
#endif

			// LuaException
			RuntimeExceptionConstructorInfo = typeof(LuaRuntimeException).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(Exception) }, null);
#if DEBUG
			if (RuntimeExceptionConstructorInfo == null)
				throw new ArgumentNullException("@2", "LuaException");
#endif

			// LuaTable
			TableSetValueKeyStringMethodInfo = typeof(LuaTable).GetMethod("SetMemberValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(string), typeof(object), typeof(bool), typeof(bool) }, null);
			TableGetValueKeyStringMethodInfo = typeof(LuaTable).GetMethod("GetMemberValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(string), typeof(bool), typeof(bool) }, null);
			TableSetValueKeyIntMethodInfo = typeof(LuaTable).GetMethod("SetArrayValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(int), typeof(object), typeof(bool) }, null);
			TableGetValueKeyIntMethodInfo = typeof(LuaTable).GetMethod("GetArrayValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(int), typeof(bool) }, null);
			TableSetValueKeyObjectMethodInfo = typeof(LuaTable).GetMethod("SetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(object), typeof(object), typeof(bool) }, null);
			TableGetValueKeyObjectMethodInfo = typeof(LuaTable).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(object), typeof(bool) }, null);
			TableSetValueKeyListMethodInfo = typeof(LuaTable).GetMethod("SetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(object[]), typeof(object), typeof(bool) }, null);
			TableGetValueKeyListMethodInfo = typeof(LuaTable).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(object[]), typeof(bool) }, null);

			TableDefineMethodLightMethodInfo = typeof(LuaTable).GetMethod("DefineMethodLight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(string), typeof(Delegate) }, null);
			TableGetCallMemberMethodInfo = typeof(LuaTable).GetMethod("GetCallMember", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableSetObjectMemberMethodInfo = typeof(LuaTable).GetMethod("SetObjectMember", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(object) }, null);

			TableEntriesFieldInfo=typeof(LuaTable).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
			TablePropertyChangedMethodInfo = typeof(LuaTable).GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(string) }, null);
			TableEntryValueFieldInfo = typeof(LuaTable).GetNestedType("LuaTableEntry", BindingFlags.NonPublic).GetField("value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);

			TableAddMethodInfo = typeof(LuaTable).GetMethod("OnAdd", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableSubMethodInfo = typeof(LuaTable).GetMethod("OnSub", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableMulMethodInfo = typeof(LuaTable).GetMethod("OnMul", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableDivMethodInfo = typeof(LuaTable).GetMethod("OnDiv", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableModMethodInfo = typeof(LuaTable).GetMethod("OnMod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TablePowMethodInfo = typeof(LuaTable).GetMethod("OnPow", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableUnMinusMethodInfo = typeof(LuaTable).GetMethod("OnUnMinus", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableIDivMethodInfo = typeof(LuaTable).GetMethod("OnIDiv", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableBAndMethodInfo = typeof(LuaTable).GetMethod("OnBAnd", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableBOrMethodInfo = typeof(LuaTable).GetMethod("OnBOr", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableBXOrMethodInfo = typeof(LuaTable).GetMethod("OnBXor", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableBNotMethodInfo = typeof(LuaTable).GetMethod("OnBNot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableShlMethodInfo = typeof(LuaTable).GetMethod("OnShl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableShrMethodInfo = typeof(LuaTable).GetMethod("OnShr", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableConcatMethodInfo = typeof(LuaTable).GetMethod("OnConcat", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableLenMethodInfo = typeof(LuaTable).GetMethod("OnLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableEqualMethodInfo = typeof(LuaTable).GetMethod("OnEqual", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableLessThanMethodInfo = typeof(LuaTable).GetMethod("OnLessThan", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableLessEqualMethodInfo = typeof(LuaTable).GetMethod("OnLessEqual", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableIndexMethodInfo = typeof(LuaTable).GetMethod("OnIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableNewIndexMethodInfo = typeof(LuaTable).GetMethod("OnNewIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
			TableCallMethodInfo = typeof(LuaTable).GetMethod("OnCall", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);

#if DEBUG
			if (TableSetValueKeyStringMethodInfo == null || TableGetValueKeyStringMethodInfo == null ||
					TableSetValueKeyIntMethodInfo == null || TableGetValueKeyIntMethodInfo == null ||
					TableSetValueKeyObjectMethodInfo == null || TableGetValueKeyObjectMethodInfo == null ||
					TableSetValueKeyListMethodInfo == null || TableGetValueKeyListMethodInfo == null ||

					TableDefineMethodLightMethodInfo == null || TableGetCallMemberMethodInfo == null || TableSetObjectMemberMethodInfo == null ||

					TableEntriesFieldInfo == null || TablePropertyChangedMethodInfo == null || TableEntryValueFieldInfo == null ||

					TableAddMethodInfo == null || TableSubMethodInfo == null || TableMulMethodInfo == null ||
					TableDivMethodInfo == null || TableModMethodInfo == null || TablePowMethodInfo == null ||
					TableUnMinusMethodInfo == null || TableIDivMethodInfo == null || TableBAndMethodInfo == null ||
					TableBOrMethodInfo == null || TableBXOrMethodInfo == null || TableBNotMethodInfo == null ||
					TableShlMethodInfo == null || TableShrMethodInfo == null || TableConcatMethodInfo == null ||
					TableLenMethodInfo == null || TableEqualMethodInfo == null || TableLessThanMethodInfo == null ||
					TableLessEqualMethodInfo == null || TableIndexMethodInfo == null || TableNewIndexMethodInfo == null ||
					TableCallMethodInfo == null)
				throw new ArgumentNullException("@3", "LuaTable");
#endif

			// LuaType
			TypeClrPropertyInfo = typeof(LuaType).GetProperty("Clr", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
			TypeGetGenericItemMethodInfo = typeof(LuaType).GetMethod("GetGenericItem", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
			TypeGetTypeMethodInfoArgIndex = typeof(LuaType).GetMethod("GetType", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(int) }, null);
			TypeGetTypeMethodInfoArgType = typeof(LuaType).GetMethod("GetType", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, null, new Type[] { typeof(Type) }, null);
			TypeTypePropertyInfo = typeof(LuaType).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
#if DEBUG
			if (TypeClrPropertyInfo == null || TypeGetGenericItemMethodInfo == null || TypeGetTypeMethodInfoArgIndex == null || TypeGetTypeMethodInfoArgType == null || TypeTypePropertyInfo == null)
				throw new ArgumentNullException("@4", "LuaType");
#endif

			// Method
			MethodConstructorInfo = typeof(LuaMethod).GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly | BindingFlags.Instance, null, new Type[] { typeof(object), typeof(MethodInfo) }, null);
			OverloadedMethodConstructorInfo = typeof(LuaOverloadedMethod).GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly | BindingFlags.Instance, null, new Type[] { typeof(object), typeof(MethodInfo[]) }, null);
			OverloadedMethodGetMethodMethodInfo = typeof(LuaOverloadedMethod).GetMethod("GetMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, new Type[] { typeof(bool), typeof(Type[]) }, null);
			MethodMethodPropertyInfo = typeof(LuaMethod).GetProperty("Method", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
			MethodNamePropertyInfo = typeof(ILuaMethod).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
			MethodTypePropertyInfo = typeof(ILuaMethod).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
			MethodInstancePropertyInfo = typeof(ILuaMethod).GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
#if DEBUG
			if (MethodConstructorInfo == null || OverloadedMethodConstructorInfo == null || OverloadedMethodGetMethodMethodInfo == null || MethodMethodPropertyInfo == null || MethodNamePropertyInfo == null || MethodTypePropertyInfo == null || MethodInstancePropertyInfo == null)
				throw new ArgumentNullException("@4.1", "LuaMethod");
#endif

			// Event
			EventConstructorInfo = typeof(LuaEvent).GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly | BindingFlags.Instance, null, new Type[] { typeof(object), typeof(EventInfo) }, null);
			AddMethodInfoPropertyInfo = typeof(LuaEvent).GetProperty("AddMethodInfo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
			RemoveMethodInfoPropertyInfo = typeof(LuaEvent).GetProperty("RemoveMethodInfo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
			RaiseMethodInfoPropertyInfo = typeof(LuaEvent).GetProperty("RaiseMethodInfo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
#if DEBUG
			if (EventConstructorInfo == null || AddMethodInfoPropertyInfo == null || RemoveMethodInfoPropertyInfo == null || RaiseMethodInfoPropertyInfo == null)
				throw new ArgumentNullException("@4.2", "LuaEvent");
#endif

			// Lua
			ParseNumberMethodInfo = typeof(Lua).GetMethod("RtParseNumber", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
			RuntimeLengthMethodInfo = typeof(Lua).GetMethod("RtLength", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
			ConvertValueMethodInfo = typeof(Lua).GetMethod("RtConvertValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(object), typeof(Type) }, null);
			GetResultValuesMethodInfo = typeof(Lua).GetMethod("RtGetResultValues", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(LuaResult), typeof(int), typeof(Type) }, null);
			CombineArrayWithResultMethodInfo = typeof(Lua).GetMethod("RtCombineArrayWithResult", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Array), typeof(LuaResult), typeof(Type) }, null);
			ConvertArrayMethodInfo = typeof(Lua).GetMethod("RtConvertArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Array), typeof(Type) }, null);
			TableSetObjectsMethod = typeof(Lua).GetMethod("RtTableSetObjects", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(LuaTable), typeof(object), typeof(int) }, null);
			ConcatStringMethodInfo = typeof(Lua).GetMethod("RtConcatString", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
			ConvertDelegateMethodInfo = typeof(Lua).GetMethod("RtConvertDelegate", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
			InitArray1MethodInfo = typeof(Lua).GetMethod("RtInitArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Type), typeof(object) }, null);
			InitArrayNMethodInfo = typeof(Lua).GetMethod("RtInitArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Type), typeof(object[]) }, null);
#if DEBUG
			if (ParseNumberMethodInfo == null || RuntimeLengthMethodInfo == null || ConvertValueMethodInfo == null || GetResultValuesMethodInfo == null ||
					CombineArrayWithResultMethodInfo == null || ConvertArrayMethodInfo == null || TableSetObjectsMethod == null || ConcatStringMethodInfo == null ||
					ConvertDelegateMethodInfo == null || InitArray1MethodInfo == null || InitArrayNMethodInfo == null)
				throw new ArgumentNullException("@5", "Lua");
#endif

			// Object
			ObjectEqualsMethodInfo = typeof(Object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
			ObjectReferenceEqualsMethodInfo = typeof(Object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
#if DEBUG
			if (ObjectEqualsMethodInfo == null || ObjectReferenceEqualsMethodInfo == null)
				throw new ArgumentNullException("@6", "Object");
#endif

			// Convert
			ConvertToStringMethodInfo = typeof(Convert).GetMethod("ToString", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(object), typeof(CultureInfo) }, null);
#if DEBUG
			if (ConvertToStringMethodInfo == null)
				throw new ArgumentNullException("@7", "Convert");
#endif

			// String
			StringEmptyFieldInfo = typeof(String).GetField("Empty", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
			StringConcatMethodInfo = typeof(String).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string[]) }, null);
			StringItemPropertyInfo = typeof(String).GetProperty("Chars", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
#if DEBUG
			if (StringEmptyFieldInfo == null || StringConcatMethodInfo == null || StringItemPropertyInfo == null)
				throw new ArgumentNullException("@8", "String");
#endif

			// CulureInfo
			CultureInvariantPropertyInfo = typeof(CultureInfo).GetProperty("InvariantCulture", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
#if DEBUG
			if (CultureInvariantPropertyInfo == null)
				throw new ArgumentNullException("@9", "CultureInfo");
#endif

			// Delegate
			DelegateMethodPropertyInfo = typeof(Delegate).GetProperty("Method", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
			CreateDelegateMethodInfo = typeof(Delegate).GetMethod("CreateDelegate", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Type), typeof(object), typeof(MethodInfo) }, null);
#if DEBUG
			if (DelegateMethodPropertyInfo == null || CreateDelegateMethodInfo == null)
				throw new ArgumentNullException("@10", "Delegate");
#endif

			ListItemPropertyInfo = typeof(List<object>).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
			ListCountPropertyInfo = typeof(List<object>).GetProperty("Count", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
#if DEBUG
			if (ListItemPropertyInfo == null || ListCountPropertyInfo == null)
				throw new ArgumentNullException("@11", "List");
#endif
		} // sctor

		#endregion

		#region -- RtParseNumber ----------------------------------------------------------

		internal static object RtParseNumber(string sNumber, int numberType)
		{
			if (sNumber == null)
				return null;
			sNumber = sNumber.Trim();
			if (sNumber.Length == 0)
				return null;

			// check for hex numbers
			if (sNumber.Length > 2 && sNumber[0] == '0' && (sNumber[1] == 'x' || sNumber[1] == 'X'))
			{
				sNumber = sNumber.Substring(2);
				numberType |= 8;
				return ParseInteger(sNumber, numberType);
			}
			else if(sNumber.Length > 3 && (sNumber[0] == '+' || sNumber[0] == '-') && sNumber[1] == '0' &&(sNumber[2] == 'x' || sNumber[2] == 'X'))
			{
				bool lNeg = sNumber[0] == '-';
				sNumber = sNumber.Substring(3);
				numberType |= 8;
				object o = ParseInteger(sNumber, numberType);
				if (lNeg)
					switch (Type.GetTypeCode(o.GetType()))
					{
						case TypeCode.Int16:
							return -(short)o;
						case TypeCode.UInt16:
							return -(int)(ushort)o;
						case TypeCode.Int32:
							return -(int)o;
						case TypeCode.UInt32:
							return -(long)(uint)o;
						case TypeCode.Int64:
							return -(long)o;
						case TypeCode.UInt64:
							return -(double)o;
						default:
							throw new InvalidOperationException();
					}
				else
					return o;
			}


			// try to convert the value
			return ParseInteger(sNumber, numberType | (int)LuaNumberFlags.NoFormatError) ?? ParseFloat(sNumber, numberType);
		} // func RtParseNumber

		#endregion

		#region -- RtConvertValue, RtConvertDelegate --------------------------------------

		/// <summary>Converts the value to the type, like NeoLua will do it.</summary>
		/// <param name="value">value, that should be converted.</param>
		/// <param name="toType">type to which the value should be converted.</param>
		/// <returns>converted value</returns>
		public static object RtConvertValue(object value, Type toType)
		{
			if (value == null)
				if (toType.IsValueType)
					return Activator.CreateInstance(toType);
				else
					return null;
			else
			{
				Type fromType = value.GetType();
				if (fromType == toType)
					return value;
				else if (fromType == typeof(LuaResult))
					return RtConvertValue(((LuaResult)value)[0], toType);
				else if (toType == typeof(object))
					return value;
				else if (toType == typeof(string))
				{
					if (fromType == typeof(bool))
						return (bool)value ? "true" : "false";
					else
					{
						foreach (MethodInfo mi in fromType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod))
						{
							if ((mi.Name == LuaEmit.csExplicit || mi.Name == LuaEmit.csImplicit) &&
								mi.ReturnType == typeof(string))
								return mi.Invoke(null, new object[] { value });
						}

						return value == null ? String.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				else if (toType.BaseType == typeof(MulticastDelegate) && toType.BaseType == fromType.BaseType)
					return RtConvertDelegate(toType, (Delegate)value);
				else if (toType.IsArray && fromType.IsArray)
					return RtConvertArray((Array)value, toType.GetElementType());
				else
				{
					TypeCode tcFrom = Type.GetTypeCode(fromType);
					TypeCode tcTo = LuaEmit.GetTypeCode(toType);
					if (tcTo == TypeCode.Object)
					{
						if (fromType.IsAssignableFrom(toType))
							return value;
						else
						{
							bool lImplicit = false;
							bool lExactTo = false;
							bool lExactFrom = false;
							MethodInfo mi = LuaEmit.FindConvertMethod(toType.GetMethods(BindingFlags.Public | BindingFlags.Static), fromType, toType, ref lImplicit, ref lExactFrom, ref lExactTo);
							if (mi != null)
							{
								if (!lExactFrom)
									value = RtConvertValue(value, mi.GetParameters()[0].ParameterType);
								value = mi.Invoke(null, new object[] { value });
								if (!lExactTo)
									value = RtConvertValue(value, toType);
							}
							return value;
						}
					}
					else if (tcTo == TypeCode.DBNull)
						return DBNull.Value;
					else
					{
						// convert from string to number through lua parser
						if (tcFrom == TypeCode.String && tcTo >= TypeCode.SByte && tcTo <= TypeCode.Decimal)
							value = RtParseNumber((string)value, (int)LuaIntegerType.Int64 | (int)LuaFloatType.Double);

						// convert to correct type
						switch (tcTo)
						{
							case TypeCode.Boolean:
								value = !Object.Equals(value, Activator.CreateInstance(toType));
								break;
							case TypeCode.Char:
								value = Convert.ToChar(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.DateTime:
								value = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.SByte:
								value = Convert.ToSByte(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Int16:
								value = Convert.ToInt16(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Int32:
								value = Convert.ToInt32(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Int64:
								value = Convert.ToInt64(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Byte:
								value = Convert.ToByte(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.UInt16:
								value = Convert.ToUInt16(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.UInt32:
								value = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.UInt64:
								value = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Single:
								value = Convert.ToSingle(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Double:
								value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.Decimal:
								value = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
								break;
							case TypeCode.String:
								value = Convert.ToString(value, CultureInfo.InvariantCulture);
								break;
							default:
								throw new InvalidOperationException("TypeCode unknown");
						}

						// check for generic and enum
						if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
							return Activator.CreateInstance(toType, value);
						else if (toType.IsEnum)
							return Enum.ToObject(toType, value);
						else
							return value;
					}
				}
			}
		} // func RtConvertValue

		internal static Delegate RtConvertDelegate(Type toType, Delegate dlg)
		{
			try
			{
				if (dlg.Method.GetType().Name == "RuntimeMethodInfo") // runtime method -> use create delegate
					return Delegate.CreateDelegate(toType, dlg.Target, dlg.Method);
				else // dynamic method -> create the delegate from the DynamicMethod.Invoke
				{
					MethodInfo mi = dlg.GetType().GetMethod("Invoke");
					return Delegate.CreateDelegate(toType, dlg, mi);
				}
			}
			catch (Exception e)
			{
				throw new LuaRuntimeException(String.Format(Properties.Resources.rsBindConversionNotDefined, dlg.GetType().Name, toType.Name), e);
			}
		} // func RtConvertDelegate

		#endregion

		#region -- RtGetResultValues, RtCombineArrayWithResult, RtConvertArray ------------

		/// <summary>Get the part of the result as an array. If there are not enough values in the array, it returns a empty array.</summary>
		/// <param name="result"></param>
		/// <param name="iStartIndex"></param>
		/// <param name="typeElementType">Type of the elements of the result array.</param>
		/// <returns></returns>
		internal static Array RtGetResultValues(LuaResult result, int iStartIndex, Type typeElementType)
		{
			object[] values = result.Values;
			int iLength = values.Length - iStartIndex;
			if (iLength > 0)
			{
				Array r = Array.CreateInstance(typeElementType, iLength);
				for (int i = 0; i < iLength; i++)
					r.SetValue(RtConvertValue(values[i + iStartIndex], typeElementType), i);
				return r;
			}
			else
				return Array.CreateInstance(typeElementType, 0); // empty array
		} // func GetResultValues

		/// <summary>Combines a array with the result.</summary>
		/// <param name="args"></param>
		/// <param name="result"></param>
		/// <param name="typeArray"></param>
		/// <returns></returns>
		internal static Array RtCombineArrayWithResult(Array args, LuaResult result, Type typeArray)
		{
			object[] values = result.Values;
			int iArgsLength = args.Length;
			int iValuesLength = values.Length;

			Array r = Array.CreateInstance(typeArray, iArgsLength + iValuesLength);

			// copy args
			for (int i = 0; i < iArgsLength; i++)
				r.SetValue(args.GetValue(i), i);

			// add the result
			for (int i = 0; i < iValuesLength; i++)
				r.SetValue(RtConvertValue(values[i], typeArray), iArgsLength + i);

			return r;
		} // func CombineArrayWithResult

		internal static Array RtConvertArray(Array src, Type typeArray)
		{
			if (src == null)
				return Array.CreateInstance(typeArray, 0);
			else
			{
				Array r = Array.CreateInstance(typeArray, src.Length);

				for (int i = 0; i < src.Length; i++)
					r.SetValue(RtConvertValue(src.GetValue(i), typeArray), i);

				return r;
			}
		} // func ConvertArray

		#endregion

		#region -- RtLength ---------------------------------------------------------------

		/// <summary>Get's the length of an value.</summary>
		/// <param name="v">Value</param>
		/// <returns>Length of the value or 0.</returns>
		public static int RtLength(object v)
		{
			if (v == null)
				return 0;
			else if (v is LuaTable)
				return ((LuaTable)v).InternLen();
			else if (v is LuaFile)
				return unchecked((int)((LuaFile)v).Length);
			else if (v is String)
				return ((String)v).Length;
			else if (v is System.IO.Stream)
				return unchecked((int)((System.IO.Stream)v).Length);
			else if (v is System.Collections.ICollection)
				return ((System.Collections.ICollection)v).Count;
			else
			{
				Type t = v.GetType();
				PropertyInfo pi;

				// search for a generic collection
				foreach (Type tInterface in t.GetInterfaces())
					if (tInterface.IsGenericType && tInterface.GetGenericTypeDefinition() == typeof(ICollection<>))
					{
						pi = tInterface.GetProperty("Count");
						return (int)pi.GetValue(v, null);
					}

				// try find a Length or Count property
				pi = t.GetProperty("Count", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, typeof(int), new Type[0], null);
				if (pi != null)
					return (int)pi.GetValue(v, null);
				pi = t.GetProperty("Length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, typeof(int), new Type[0], null);
				if (pi != null)
					return (int)pi.GetValue(v, null);

				return 0;
			}
		} // func RtLength

		#endregion

		#region -- RtTableSetObjects ------------------------------------------------------

		internal static object RtTableSetObjects(LuaTable t, object value, int iStartIndex)
		{
			if (value != null && value is LuaResult)
			{
				LuaResult v = (LuaResult)value;

				for (int i = 0; i < v.Count; i++)
					t.SetArrayValue(iStartIndex++, v[i], true);
			}
			else
				t.SetArrayValue(iStartIndex, value, true);
			return t;
		} // func RtTableSetObjects

		#endregion

		#region -- RtInvoke ---------------------------------------------------------------

		internal static bool IsCallable(object ld)
		{
			return ld is Delegate || ld is ILuaMethod || ld is IDynamicMetaObjectProvider;
		} // func IsCallable

		internal static object RtInvokeSite(Func<CallInfo, CallSiteBinder> createInvokeBinder, Action<CallInfo, CallSite> updateCache, object[] args)
		{
			if (args[0] == null)
			{
				// create the delegate
				Type[] signature = new Type[args.Length + 1];
				signature[0] = typeof(CallSite); // CallSite
				for (int i = 1; i < args.Length; i++) // target + arguments
					signature[i] = typeof(object);
				signature[signature.Length - 1] = typeof(object); // return type

				// create a call site
				CallInfo callInfo = new CallInfo(args.Length - 1);
				CallSite site;
				args[0] = site = CallSite.Create(Expression.GetFuncType(signature), createInvokeBinder(callInfo));
				if (updateCache != null)
					updateCache(callInfo, site);
			}

			// call the site
			object o = args[0];
			FieldInfo fi = o.GetType().GetField("Target", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);
			Delegate dlg = (Delegate)fi.GetValue(o);
			return new LuaResult(dlg.DynamicInvoke(args));
		} // func RtInvokeSite

		internal static object RtInvokeSite(object target, params object[] args)
		{
			return RtInvokeSite(null, callInfo => new Lua.LuaInvokeBinder(null, callInfo), null, target, args);
		} // func RtInvokeSite

		internal static object RtInvokeSite(CallSite site, Func<CallInfo, CallSiteBinder> createInvokeBinder, Action<CallInfo, CallSite> updateCache, object target, params object[] args)
		{
			// expand args for callsite and target
			object[] newArgs = new object[args.Length + 2];
			newArgs[0] = site;
			newArgs[1] = target;
			Array.Copy(args, 0, newArgs, 2, args.Length);

		  // call site
			return RtInvokeSite(callInfo => new Lua.LuaInvokeBinder(null, callInfo), null, newArgs);
		} // func RtInvokeSite

		#endregion

		#region -- RtConcatString ---------------------------------------------------------

		private static string RtConcatStringTable(object[] args, int iIndex)
		{
			if (iIndex >= args.Length - 1)
				return (string)RtConvertValue(args[iIndex], typeof(string));
			else if (args[iIndex] is LuaTable)
				return (string)RtConvertValue(((LuaTable)args[iIndex]).InternConcat(RtConcatStringTable(args, iIndex + 1)), typeof(string));
			else
				return (string)RtConvertValue(args[iIndex], typeof(string)) + RtConcatStringTable(args, iIndex + 1);
		} // func RtConcatStringTable

		internal static string RtConcatString(object[] args)
		{
			if (Array.Exists(args, a => a is LuaTable)) // do we have a table, than we use the metatable
			{
				return RtConcatStringTable(args, 0);
			}
			else
			{
				string[] strings = new string[args.Length];
				for (int i = 0; i < args.Length; i++)
					strings[i] = (string)RtConvertValue(args[i], typeof(string));
				return String.Concat(strings);
			}
		} // func RtConcatString

		#endregion

		#region -- RtInitArray ------------------------------------------------------------

		internal static object RtInitArray(Type elementType, object value)
		{
			if (value is LuaTable) // only the array part
			{
				LuaTable t = (LuaTable)value;
				int iLength = t.Length;

				// create the array
				Array r = Array.CreateInstance(elementType, iLength);

				// copy the values
				for (int i = 0; i < iLength; i++)
					r.SetValue(Lua.RtConvertValue(t[i + 1], elementType), i);

				return r;
			}
			else if (value is System.Collections.ICollection) // convert a collection to an array
			{
				System.Collections.ICollection c = (System.Collections.ICollection)value;

				// create the array an copy the values
				Array r = Array.CreateInstance(elementType, c.Count);
				c.CopyTo(r, 0);

				return r;
			}
			else // create a zero-value array
			{
				Array r = Array.CreateInstance(elementType, 1);
				r.SetValue(value, 0);
				return r;
			}
		} // func RtInitArray

		internal static object RtInitArray(Type elementType, object[] values)
		{
			Array r = Array.CreateInstance(elementType, values.Length);
			if (values.Length > 0)
			{
				for (int i = 0; i < values.Length; i++)
					r.SetValue(Lua.RtConvertValue(values[i], elementType), i);
			}
			return r;
		} // func RtInitArray

		#endregion

		#region -- Enumerator -------------------------------------------------------------

		private readonly static Func<object, object, LuaResult> funcLuaEnumIterator = new Func<object, object, LuaResult>(LuaEnumIteratorImpl);

		private static LuaResult LuaEnumIteratorImpl(object s, object c)
		{
			System.Collections.IEnumerator e = (System.Collections.IEnumerator)s;
			if (e.MoveNext())
				return new LuaResult(e.Current);
			else
				return LuaResult.Empty;
		} // func LuaEnumIteratorImpl

		/// <summary>Convert IEnumerator's to lua enumerator-functions.</summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static LuaResult GetEnumIteratorResult(System.Collections.IEnumerator e)
		{
			return new LuaResult(funcLuaEnumIterator, e, null);
		} // func GetEnumIteratorResult

		#endregion
	} // class Lua

	#endregion
}
