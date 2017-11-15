﻿using VisualMutator.Tests.Operators;

namespace VisualMutator.Tests.Mutations
{
    #region

    using System.Linq;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Microsoft.Cci;
    using Model.CoverageFinder;
    using NUnit.Framework;

    #endregion

    [TestFixture("Ns.Class.Method1<T>()")]
    [TestFixture("Ns.Class.InnerClass<K, Y>.Met2<R>(K, System.Int32)")]
    [TestFixture("Ns.Class<S>.Inner.Inner2.Met2<R, L>()")]
    [TestFixture("Ns.Class<S>.Inner.Inner2..ctor()")]
    [TestFixture("Ns.Class<S>..ctor(System.Int32)")]
    [TestFixture("Ns.Class.Method1(System.String)")]
    [TestFixture("Ns.Class.Method1()")]
    [TestFixture("Ns.Class.Method2<U>(System.Collections.Generic.IEnumerable<U>)")]
   // [Explicit]
    public class CciMethodMatchingTests
    {
        private const string code = @"

    namespace Ns
    {
        using System.Collections.Generic;
        public class Class
        {
            public void Method2<U>(IEnumerable<U> e) { }
            public void Method1() { }
            public void Method1(string s) { }
            public static void Method1<T>() { }

            class InnerClass<K, Y>
            {
                 public void Met2<R>(K k, int i) { }
            }
        }

        public class Class<S>
        {
            Class(int i){}
            class Inner
            {
                class Inner2
                {
                    public void Met2<R, L>() { }
                }
            }
        }
    }

";

        private readonly MethodIdentifier _context;
        private IModule _module;

        public CciMethodMatchingTests(string method)
        {
            _context = new MethodIdentifier(method);
        }

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
                {
                    Layout = new SimpleLayout()
                });

            _module = MutationTestsHelper.CreateModuleFromCode(code).Module.Module;
        }

        [Test]
        public void ShouldFindMethodByIdentifier()
        {
            var searcher = new CciMethodMatcher(_context);
            var methods = _module.GetAllTypes().SelectMany(t => t.Methods).ToList();
            var methodsMatch = methods.Where(searcher.Matches).ToList();

            Assert.IsNotNull(methodsMatch.Single());
        }

        [Test]
        public void ShouldFindTypeByIdentifier()
        {
            var searcher = new CciMethodMatcher(_context);
            var types = _module.GetAllTypes();
            INamedTypeDefinition method = types.SingleOrDefault(searcher.Matches);

            Assert.IsNotNull(method);
        }
    }
}