﻿#region Using

using Nancy;
using Nancy.ModelBinding;
using System.Dynamic;
using System.Reflection;
using ThumbsUp.Domain;

#endregion

namespace ThumbsUp.Service.Module
{
	public class _BaseModule : NancyModule
	{
		public class ParameterBag
		{
			public string SingleUseToken { get; set; }
			public string ThumbKey { get; set; }
			public string Name { get; set; }
			public string Id { get; set; }
			public string UserName { get; set; }
			public string Password { get; set; }
			public string Email { get; set; }
			public string Token { get; set; }
			public string Code { get; set; }

			public bool AreMissing(params string[] names) 
			{
				var type = this.GetType();
				foreach(var name in names)
				{
					var property = type.GetProperty(name);
					if (property != null) 
					{
						var value = (string)property.GetValue(this);
						if (string.IsNullOrWhiteSpace(value)) return true;
					}
				}
				return false;
			}
		}

		public ParameterBag Params;
		public _BaseModule() : this("")
		{}

		public _BaseModule(string modulePath) : base(modulePath)
		{
			Before += ctx => 
			{			
				Params = this.Bind<ParameterBag>();
				return null; 
			};
		}
	}
}
