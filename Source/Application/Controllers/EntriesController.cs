using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Extensions;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public class EntriesController : SiteController
	{
		#region Constructors

		public EntriesController(IFacade facade) : base(facade) { }

		#endregion

		#region Methods

		protected internal virtual IEntryNode GetEntryNode(string distinguishedName, IEntryNode entryNode)
		{
			if(string.IsNullOrEmpty(distinguishedName) || entryNode == null)
				return null;

			const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

			if(!distinguishedName.EndsWith(entryNode.Entry.DistinguishedName, comparison))
				return null;

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(distinguishedName.Equals(entryNode.Entry.DistinguishedName, comparison))
				return entryNode;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return this.GetEntryNode(distinguishedName, entryNode.Children.FirstOrDefault(child => distinguishedName.EndsWith(child.Key, comparison)).Value);
		}

		public virtual async Task<IActionResult> Index(string id)
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = new EntriesViewModel();
			var tree = this.Facade.DirectoryInformation.Tree;

			var entryNode = !string.IsNullOrEmpty(id) ? this.GetEntryNode(id, tree) : tree;

			// ReSharper disable InvertIf
			if(entryNode != null)
			{
				model.EntryNode = entryNode;

				foreach(var ancestor in entryNode.Ancestors())
				{
					model.Ancestors.Add(ancestor);
				}
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(this.View(model));
		}

		#endregion
	}
}