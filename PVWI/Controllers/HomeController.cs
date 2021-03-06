﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
