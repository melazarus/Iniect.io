﻿using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTests.Iniect.io.TestDomain;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class InjectTests : SuperTestClass
    {
        [TestMethod]
        public void InjectProperties()
        {
            var a = new ClassA();
            Assert.IsNull(a.DependencyB);
            Factory.Inject(a);
            Assert.IsNotNull(a.DependencyB);
        }
    }
}