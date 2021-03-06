﻿using Harmony;
using HarmonyTests.Assets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HarmonyTests
{
	[TestClass]
	public class TestTraverse_Methods
	{
		[TestMethod]
		public void Traverse_Missing_Method()
		{
			var instance = new TraverseMethods_Instance();
			var trv = Traverse.Create(instance);

			string methodSig1 = "";
			try
			{
				trv.Method("FooBar", new object[] { "hello", 123 });
			}
			catch (MissingMethodException e)
			{
				methodSig1 = e.Message;
			}
			Assert.AreEqual("FooBar(System.String, System.Int32)", methodSig1);

			string methodSig2 = "";
			try
			{
				var types = new Type[] { typeof(string), typeof(int) };
				trv.Method("FooBar", types, new object[] { "hello", 123 });
			}
			catch (MissingMethodException e)
			{
				methodSig2 = e.Message;
			}
			Assert.AreEqual("FooBar(System.String, System.Int32)", methodSig2);
		}

		[TestMethod]
		public void Traverse_Method_Instance()
		{
			var instance = new TraverseMethods_Instance();
			var trv = Traverse.Create(instance);

			instance.Method1_called = false;
			var mtrv1 = trv.Method("Method1");
			Assert.AreEqual(null, mtrv1.GetValue());
			Assert.AreEqual(true, instance.Method1_called);

			var mtrv2 = trv.Method("Method2", new object[] { "arg" });
			Assert.AreEqual("argarg", mtrv2.GetValue());
		}

		[TestMethod]
		public void Traverse_Method_Static()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Static));
			var mtrv = trv.Method("StaticMethod", new object[] { 6, 7 });
			Assert.AreEqual(42, mtrv.GetValue<int>());
		}

		[TestMethod]
		public void Traverse_Method_VariableArguments()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_VarArgs));

			Assert.AreEqual(30, trv.Method("Test1", 10, 20).GetValue<int>());
			Assert.AreEqual(60, trv.Method("Test2", 10, 20, 30).GetValue<int>());

			// Calling varargs methods directly won't work. Use parameter array instead
			// Assert.AreEqual(60, trv.Method("Test3", 100, 10, 20, 30).GetValue<int>());
			Assert.AreEqual(6000, trv.Method("Test3", 100, new int[] { 10, 20, 30 }).GetValue<int>());
		}

		[TestMethod]
		public void Traverse_Method_RefParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithRefParameter", types, parameters);
			Assert.AreEqual("ok", mtrv1.GetValue<string>());
			Assert.AreEqual("hello", parameters[0]);
		}

		[TestMethod]
		public void Traverse_Method_OutParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithOutParameter", types, parameters);
			Assert.AreEqual("ok", mtrv1.GetValue<string>());
			Assert.AreEqual("hello", parameters[0]);
		}
	}
}