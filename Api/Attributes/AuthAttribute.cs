using Api.Constants;
using CED.Framework.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Api.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class AuthAttribute : AuthorizeAttribute
	{
		private List<string> _ActionTypes { get; } = new List<string>();
		private List<string> _Roles { get; } = new List<string>();
		private JwtSecurityToken DecodedToken { get; set; }

		private readonly IClaimsReader _claimsReader = new ClaimsReader();

		public AuthAttribute(params string[] parameters)
		{
			var rolesList = typeof(AuthRoles).GetFields(BindingFlags.Static | BindingFlags.Public).Select(info => info.Name).ToList();
			var actionTypesList = typeof(AuthActionTypes).GetFields(BindingFlags.Static | BindingFlags.Public).Select(info => info.Name).ToList();

			foreach (var parameter in parameters)
			{
				if (rolesList.Contains(parameter))
				{
					_Roles.Add(parameter);
				}
				else if (actionTypesList.Contains(parameter))
				{
					_ActionTypes.Add(parameter);
				}
			}
		}

		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if (actionContext.Request.Headers.Authorization == null)
			{
				return false;
			}

			DecodedToken = new JwtSecurityToken(actionContext.Request.Headers.Authorization.Parameter);
			return _Roles.Any(r => HasRole(actionContext, r));
		}

		private bool HasRole(HttpActionContext actionContext, string role)
		{
			if (!string.IsNullOrEmpty(actionContext.Request.Headers.Authorization.Parameter))
			{
				string scheme = actionContext.Request.Headers.Authorization.Scheme;
				string token = actionContext.Request.Headers.Authorization.Parameter;
				if (!string.IsNullOrEmpty(token))
				{
					string rolesProviderUri =
						_claimsReader.GetClaimValue(actionContext.RequestContext.Principal, "RolesProviderUri");

					using (var httpClient = new HttpClient())
					{
						httpClient.DefaultRequestHeaders.Authorization =
							new AuthenticationHeaderValue(scheme, token);
						var rolesResponse = httpClient.GetAsync($"{rolesProviderUri}/{role}").Result;
						rolesResponse.EnsureSuccessStatusCode();
						string rolesContent = rolesResponse.Content.ReadAsStringAsync().Result;
						var isInRole = JsonConvert.DeserializeObject<bool>(rolesContent);
						return isInRole;
					}

				}
			}

			return false;
		}

		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			var userNameValue = DecodedToken?.Claims.FirstOrDefault(x => x.Type == "upn")?.Value;
			StringContent content;

			if (!_Roles.Any() || !_ActionTypes.Any())
				content = new StringContent("Access is not allowed for any role");
			else
				content = new StringContent(
					$"The role(s) {string.Join(", ", _Roles)} and permission(s) for {string.Join(", ", _ActionTypes)} could not be found in the claims of the principal '{userNameValue}'.");

			actionContext.Response = new HttpResponseMessage
			{
				Content = content,
				StatusCode = HttpStatusCode.Unauthorized,
			};
		}
	}
}