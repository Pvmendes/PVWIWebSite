﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PVWIContext.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace PVWI.DAO
{
    using System.Data.Entity;

    using PVWI.Entities;

    /// <summary>
    /// Contexto usado para consultas no banco de dados para faturas.
    /// </summary>
    public class PvwiContext : DbContext
    {
        /// <summary>
        /// Usuários do sistema que podem ser inclusos em uma fatura.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Tabela de items dentro de uma fatura.
        /// </summary>
        public DbSet<CreditCardBillItem> BillItems { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///             before the model has been locked down and used to initialize the context.  The default
        ///             implementation of this method does nothing, but it can be overridden in a derived class
        ///             such that the model can be further configured before it is locked down.
        /// </summary>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        ///             is created.  The model for that context is then cached and is for all further instances of
        ///             the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///             property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///             More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///             classes directly.
        /// </remarks>
        /// <param name="modelBuilder">
        /// The builder that defines the model for the context being created. 
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditCardBillItem>().HasRequired(x => x.User);
            modelBuilder.Entity<CreditCardBillItem>().HasRequired(x => x.CreditCardBill);
        }
    }
}