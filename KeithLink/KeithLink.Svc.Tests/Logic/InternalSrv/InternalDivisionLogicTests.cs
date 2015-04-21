using KeithLink.Svc.Core.Interface.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.InternalSrv
{
	public class InternalDivisionLogicTests
	{
		private readonly IInternalDivisionLogic divisionLogic;

		public InternalDivisionLogicTests()
		{
			var container = DependencyMap.Build();
			divisionLogic = container.Resolve<IInternalDivisionLogic>();
		}

		[TestMethod]
		public void GetDivisions()
		{
			var divisions = divisionLogic.ReadBranchSupport();
			Assert.IsNotNull(divisions);
		}
	}
}
