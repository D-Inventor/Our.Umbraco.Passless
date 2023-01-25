﻿using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;

namespace UmbracoFidoLogin.Credentials.Endpoints;

[UmbracoRequireHttps]
[DisableBrowserCache]
[Area(UmbracoFidoConstants.AreaName)]
public class CredentialsOptionsController : UmbracoAuthorizedController
{
    private readonly IFido2 fido2;

    public CredentialsOptionsController(IFido2 fido2)
    {
        this.fido2 = fido2;
    }
    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity is null)
        {
            throw new InvalidOperationException("Unexpected, user claims not initialized");
        }

        var useremail = User.Identity.GetEmail();

        var user = new Fido2User
        {
            DisplayName = useremail,
            Name = useremail,
            Id = Encoding.UTF8.GetBytes(useremail)
        };

        var existingKeys = new List<PublicKeyCredentialDescriptor>();//TODO: read from DB

        var authenticatorSelection = new AuthenticatorSelection
        {
            RequireResidentKey = true,
            UserVerification = UserVerificationRequirement.Preferred
        }; //TODO: get this from config.

        authenticatorSelection.AuthenticatorAttachment = AuthenticatorAttachment.CrossPlatform; //TODO: Make this toggleable when signing up. For now we only use WindowsID

        var exts = new AuthenticationExtensionsClientInputs()
        {
            Extensions = true,
            UserVerificationMethod = true,
        };

        var options = fido2.RequestNewCredential(user, existingKeys, authenticatorSelection, AttestationConveyancePreference.None, exts);

        // 4. Temporarily store options, session/in-memory cache/redis/db
        HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

        return new JsonResult(options);
    }
}
