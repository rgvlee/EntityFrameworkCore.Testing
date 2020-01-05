using System;
using EntityFrameworkCore.Testing.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TypeExtensionsTests
    {
        [Test]
        public void GetDefaultValue_Guid_ReturnsDefaultGuid()
        {
            Assert.That(typeof(Guid).GetDefaultValue(), Is.EqualTo(default(Guid)));
        }

        [Test]
        public void GetDefaultValue_Int_ReturnsDefaultInt()
        {
            Assert.That(typeof(int).GetDefaultValue(), Is.EqualTo(default(int)));
        }

        [Test]
        public void GetDefaultValue_Bool_ReturnsDefaultInt()
        {
            Assert.That(typeof(bool).GetDefaultValue(), Is.EqualTo(default(bool)));
        }

        [Test]
        public void GetDefaultValue_String_ReturnsDefaultInt()
        {
            Assert.That(typeof(string).GetDefaultValue(), Is.EqualTo(default(string)));
        }

        [Test]
        public void GetDefaultValue_TypeWithParameterlessConstructor_ReturnsDefaultInt()
        {
            Assert.That(typeof(TypeWithParameterlessConstructor).GetDefaultValue(), Is.Null);
        }

        [Test]
        public void HasParameterlessConstructor_TypeWithParameterlessConstructor_ReturnsTrue()
        {
            Assert.That(typeof(TypeWithParameterlessConstructor).HasParameterlessConstructor(), Is.True);
        }

        [Test]
        public void HasParameterlessConstructor_TypeWithoutParameterlessConstructor_ReturnsFalse()
        {
            Assert.That(typeof(TypeWithoutParameterlessConstructor).HasParameterlessConstructor(), Is.False);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithDbContextOptionsConstructor_DbContextOptions_ReturnsTrue()
        {
            Assert.That(typeof(TypeWithDbContextOptionsConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions)), Is.True);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithDbContextOptionsConstructor_OpenGenericDbContextOptions_ReturnsTrue()
        {
            Assert.That(typeof(TypeWithDbContextOptionsConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions<>)), Is.True);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithoutParameterlessConstructor_DbContextOptions_ReturnsFalse()
        {
            Assert.That(typeof(TypeWithoutParameterlessConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions)), Is.False);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithGenericDbContextOptionsConstructor_GenericDbContextOptions_ReturnsTrue()
        {
            Assert.That(typeof(TypeWithGenericDbContextOptionsConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions<TypeWithGenericDbContextOptionsConstructor>)), Is.True);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithGenericDbContextOptionsConstructor_OpenGenericDbContextOptions_ReturnsFalse()
        {
            Assert.That(typeof(TypeWithGenericDbContextOptionsConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions<>)), Is.False);
        }

        [Test]
        public void HasConstructorWithParameterOfTypeForTypeWithGenericDbContextOptionsConstructor_DbContextOptions_ReturnsFalse()
        {
            Assert.That(typeof(TypeWithGenericDbContextOptionsConstructor).HasConstructorWithParameterOfType(typeof(DbContextOptions)), Is.False);
        }

        [Test]
        public void GetConstructor_ParentAndChildParameterTypes_ReturnsConstructor()
        {
            var type = typeof(TypeWithParentParameterConstructor);
            var ci1 = type.GetConstructor(new[] {typeof(ParentParameter)});
            var ci2 = type.GetConstructor(new[] {typeof(ChildParameter)});
            var ci3 = type.GetConstructor(new[] {typeof(string)});

            Assert.Multiple(() =>
            {
                Assert.That(ci3, Is.Null); //Control test
                Assert.That(ci1, Is.Not.Null);
                Assert.That(ci2, Is.Not.Null);
                Assert.That(ci1, Is.EqualTo(ci2));
            });
        }

        public class TypeWithParameterlessConstructor { }

        public class TypeWithoutParameterlessConstructor
        {
            public TypeWithoutParameterlessConstructor(Guid id) { }
        }

        public class TypeWithDbContextOptionsConstructor : DbContext
        {
            public TypeWithDbContextOptionsConstructor(DbContextOptions dbContextOptions) { }
        }

        public class TypeWithGenericDbContextOptionsConstructor : DbContext
        {
            public TypeWithGenericDbContextOptionsConstructor(DbContextOptions<TypeWithGenericDbContextOptionsConstructor> dbContextOptions) { }
        }

        public class ParentParameter { }

        public class ChildParameter : ParentParameter { }

        public class TypeWithParentParameterConstructor
        {
            public TypeWithParentParameterConstructor(ParentParameter parentParameter) { }
        }
    }
}