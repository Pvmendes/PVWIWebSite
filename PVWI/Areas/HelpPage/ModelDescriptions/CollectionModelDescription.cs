// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionModelDescription.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// The collection model description.
    /// </summary>
    public class CollectionModelDescription : ModelDescription
    {
        /// <summary>
        /// Gets or sets the element description.
        /// </summary>
        public ModelDescription ElementDescription { get; set; }
    }
}