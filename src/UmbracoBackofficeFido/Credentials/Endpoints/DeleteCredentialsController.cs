﻿using Microsoft.AspNetCore.Mvc;
using System.Text;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;
using UmbracoFidoLogin.Credentials.Services;

namespace UmbracoFidoLogin.Credentials.Endpoints;

[UmbracoRequireHttps]
[DisableBrowserCache]
[Area(UmbracoFidoConstants.AreaName)]
public class DeleteCredentialsController : UmbracoAuthorizedController
{
    private readonly ICredentialsService credentialsService;

    public DeleteCredentialsController(ICredentialsService credentialsService)
    {
        this.credentialsService = credentialsService;
    }

    [HttpPost]
    public async Task<IActionResult> Index(string id)
    {
        var userEmail = User.Identity?.GetEmail();
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new InvalidOperationException("Unexpected: User email is null");
        }

        await credentialsService.DeleteCredentialsAsync(userEmail, Convert.FromHexString(id));

        return Ok();
    }
}
