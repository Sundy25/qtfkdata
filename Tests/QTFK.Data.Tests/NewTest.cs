using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Models;
using System.Linq;
using QTFK.Services.Factories;
using QTFK.Services.DBIO;
using QTFK.Services;
using QTFK.Services.FilterParsers;
using System.Reflection;
using QTFK.Data.Tests.Services;
using QTFK.Services.RepositoryBuilders;
using System.Collections.Generic;
using System;
using QTFK.Services.CompilerWrappers;
using SampleLibrary.Services;
using SampleLibrary.Models;
using QTFK.Extensions.Collections.Casting;
using QTFK.Extensions.Assemblies;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class NewTest
    {
        private ISampleRepository prv_dependencyInjectionFake_Build()
        {
            ICompilerWrapper compilerWrapper;
            IQueryFactory lowLevelqueryFactory;
            IQueryFactory<SampleClass> queryFactory;
            IEnumerable<IMethodParser> methodParsers;
            IEnumerable<IQueryFilterFactory> filterFactories;
            IDBIO db;
            IRepositoryBuilder repositoryBuilder;
            IRepositoryExplorer repositoryExplorer;
            Type sampleRepositoryInterface;

            db = new OleDBIO("booooooom");
            lowLevelqueryFactory = new OleDBQueryFactory(db);

            methodParsers = new IMethodParser[]
            {
                new ByParamEqualsFilterParser(),
                new ByParamBetweenFilterParser(),
            };

            filterFactories = new IQueryFilterFactory[]
            {
                lowLevelqueryFactory.As<IQueryFilterFactory>()
            };

            queryFactory = new DefaultQueryFactory<SampleClass>(lowLevelqueryFactory, filterFactories);

            compilerWrapper = new CompilerWrapper();
            repositoryBuilder = new DefaultRepositoryBuilder(compilerWrapper);
            repositoryExplorer = new FakeRepositoryExplorer();

            sampleRepositoryInterface = repositoryExplorer
                .GetInterfaceTypes()
                .FirstOrDefault()
                //.FirstOrDefault(t => t.FullName == typeof(ISampleRepository).FullName)
                ;

            //Assembly repoAssembly = repositoryBuilder.Build(sampleRepositoryInterface, queryFactory, methodParsers);
            //var repo = repoAssembly.CreateInstance()
            return new SampleRepository(queryFactory, methodParsers);
            //return repo;
        }

        private IMinimalRepository builderForMinimalReporitory()
        {
            IRepositoryBuilder repositoryBuilder;
            ICompilerWrapper compilerWrapper;
            IMinimalRepository repository;
            IQueryFactory lowLevelqueryFactory;
            IQueryFactory<SampleClass> queryFactory;
            Assembly assembly;
            Type interfaceType;
            IDBIO db;
            IEnumerable<IQueryFilterFactory> filterFactories;
            IEnumerable<IMethodParser> methodParsers;
            object[] constructorParameters;

            compilerWrapper = new CompilerWrapper();
            repositoryBuilder = new DefaultRepositoryBuilder(compilerWrapper);

            db = new OleDBIO("booooooom");
            lowLevelqueryFactory = new OleDBQueryFactory(db);
            filterFactories = new IQueryFilterFactory[] { lowLevelqueryFactory.As<IQueryFilterFactory>() };

            queryFactory = new DefaultQueryFactory<SampleClass>(lowLevelqueryFactory, filterFactories);

            methodParsers = new IMethodParser[]
            {
                new ByParamEqualsFilterParser(),
                new ByParamBetweenFilterParser(),
            };

            interfaceType = typeof(IMinimalRepository);
            assembly = repositoryBuilder.Build(interfaceType);
            constructorParameters = new object[] { queryFactory, methodParsers };
            repository = assembly.createAssignableInstance(interfaceType, constructorParameters) as IMinimalRepository;

            return repository;
        }

        [TestMethod]
        public void TestMethod1()
        {
            ISampleRepository repo = prv_dependencyInjectionFake_Build();

            RepositoryOperationResult result;

            var items = repo.Get();

            Assert.IsFalse(items.Any());

            var item = new SampleClass
            {
                Name = "pepe",
                WalletCash = 666m
            };

            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Added, result);
            Assert.IsNotNull(item.ID);

            item.WalletCash = 3.14159265m;
            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Updated, result);

            var item2 = repo
                .Get()
                .Where(i => i.ID == item.ID)
                .Single()
                ;

            var item3 = repo
                .GetByName("pepe")
                ;

            Assert.AreNotSame(item, item2);
            Assert.AreEqual(666m, item2.WalletCash);

            Assert.AreNotSame(item, item3);
            Assert.AreEqual(666m, item2.WalletCash);
            Assert.AreSame(item.ID, item3.ID);

            var itemsBetween = repo
                .GetByWalletCashBetween(500m, 1000m)
                .ToArray()
                ;

            Assert.AreEqual(1, itemsBetween.Count());
        }

        [TestMethod]
        public void MinimalRepository_test()
        {
            IMinimalRepository repo = builderForMinimalReporitory();

            RepositoryOperationResult result;

            var items = repo.Get();

            Assert.IsFalse(items.Any());

            var item = new SampleClass
            {
                Name = "pepe",
                WalletCash = 666m
            };

            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Added, result);
            Assert.IsNotNull(item.ID);

            item.WalletCash = 3.14159265m;
            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Updated, result);

            var item2 = repo
                .Get()
                .Where(i => i.ID == item.ID)
                .Single()
                ;

            Assert.AreNotSame(item, item2);
            Assert.AreEqual(666m, item2.WalletCash);
        }

    }
}
