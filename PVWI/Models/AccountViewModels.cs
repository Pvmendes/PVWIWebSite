// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountViewModels.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Models
{
    using System.Collections.Generic;

    // Models returned by AccountController actions.

    /// <summary>
    /// The external login view model.
    /// </summary>
    public class ExternalLoginViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string State { get; set; }
    }

    /// <summary>
    /// The manage info view model.
    /// </summary>
    public class ManageInfoViewModel
    {
        /// <summary>
        /// Gets or sets the local login provider.
        /// </summary>
        public string LocalLoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the logins.
        /// </summary>
        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        /// <summary>
        /// Gets or sets the external login providers.
        /// </summary>
        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    /// <summary>
    /// The user info view model.
    /// </summary>
    public class UserInfoViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has registered.
        /// </summary>
        public bool HasRegistered { get; set; }

        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        public string LoginProvider { get; set; }
    }

    /// <summary>
    /// The user login info view model.
    /// </summary>
    public class UserLoginInfoViewModel
    {
        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public string ProviderKey { get; set; }
    }
}
