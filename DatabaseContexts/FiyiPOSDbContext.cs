using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.EntitiesConfiguration;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.EntitiesConfiguration;
using FiyiPOS.Areas.System.FailureBack.Entities;
using FiyiPOS.Areas.System.FailureBack.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.DatabaseContexts
{
    public class FiyiPOSDbContext(DbContextOptions<FiyiPOSDbContext> options) : DbContext(options)
    {

        //System
        public DbSet<Failure> Failure { get; set; }

        //FiyiPOS
        public DbSet<Client> Client { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<SystemUser> SystemUser { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }
        public DbSet<SaleStatus> SaleStatus { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<SaleDetail> SaleDetail { get; set; }
        public DbSet<SalePayment> SalePayment { get; set; }
        public DbSet<CashCut> CashCut { get; set; }
        public DbSet<CashCutDetail> CashCutDetail { get; set; }
        public DbSet<InventoryMovement> InventoryMovement { get; set; }
        public DbSet<Credit> Credit { get; set; }
        public DbSet<CreditPayment> CreditPayment { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public DbSet<SystemUserStore> SystemUserStore { get; set; }
        public DbSet<ReturnSale> ReturnSale { get; set; }
        public DbSet<ReturnSaleDetail> ReturnSaleDetail { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<PromotionDetail> PromotionDetail { get; set; }
        public DbSet<Notepad> Notepad { get; set; }
        public DbSet<Attendance>     Attendance     { get; set; }
        public DbSet<CashMovement>  CashMovement   { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<AfipAccessTicket> AfipAccessTicket { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<AfipLog> AfipLog { get; set; }
        public DbSet<InternalConsumption> InternalConsumption { get; set; }
        public DbSet<InternalConsumptionDetail> InternalConsumptionDetail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //System
            modelBuilder.ApplyConfiguration(new FailureConfiguration());
            modelBuilder.Entity<Failure>().ToTable("System.Failure");

            //FiyiPOS — Common (toda variante: Físico, Online, Restó, Canchas)
            modelBuilder.ApplyConfiguration(new StoreConfiguration());
            modelBuilder.Entity<Store>().ToTable("FiyiPOS.Common.Store");
            modelBuilder.ApplyConfiguration(new SystemUserConfiguration());
            modelBuilder.Entity<SystemUser>().ToTable("FiyiPOS.Common.SystemUser");
            modelBuilder.ApplyConfiguration(new SystemUserStoreConfiguration());
            modelBuilder.Entity<SystemUserStore>().ToTable("FiyiPOS.Common.SystemUserStore");
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.Entity<Client>().ToTable("FiyiPOS.Common.Client");
            modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
            modelBuilder.Entity<PaymentMethod>().ToTable("FiyiPOS.Common.PaymentMethod");
            modelBuilder.ApplyConfiguration(new SaleStatusConfiguration());
            modelBuilder.Entity<SaleStatus>().ToTable("FiyiPOS.Common.SaleStatus");
            modelBuilder.ApplyConfiguration(new SaleConfiguration());
            modelBuilder.Entity<Sale>().ToTable("FiyiPOS.Common.Sale");
            modelBuilder.ApplyConfiguration(new SaleDetailConfiguration());
            modelBuilder.Entity<SaleDetail>().ToTable("FiyiPOS.Common.SaleDetail");
            modelBuilder.ApplyConfiguration(new SalePaymentConfiguration());
            modelBuilder.Entity<SalePayment>().ToTable("FiyiPOS.Common.SalePayment");
            modelBuilder.ApplyConfiguration(new CashCutConfiguration());
            modelBuilder.Entity<CashCut>().ToTable("FiyiPOS.Common.CashCut");
            modelBuilder.ApplyConfiguration(new CashCutDetailConfiguration());
            modelBuilder.Entity<CashCutDetail>().ToTable("FiyiPOS.Common.CashCutDetail");
            modelBuilder.ApplyConfiguration(new CashMovementConfiguration());
            modelBuilder.Entity<CashMovement>().ToTable("FiyiPOS.Common.CashMovement");
            modelBuilder.ApplyConfiguration(new ConfigConfiguration());
            modelBuilder.Entity<Config>().ToTable("FiyiPOS.Common.Config");
            modelBuilder.ApplyConfiguration(new CompanySettingsConfiguration());
            modelBuilder.Entity<CompanySettings>().ToTable("FiyiPOS.Common.CompanySettings");
            modelBuilder.ApplyConfiguration(new NotepadConfiguration());
            modelBuilder.Entity<Notepad>().ToTable("FiyiPOS.Common.Notepad");
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.Entity<Attendance>().ToTable("FiyiPOS.Common.Attendance");
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.Entity<Invoice>().ToTable("FiyiPOS.Common.Invoice");
            modelBuilder.ApplyConfiguration(new AfipAccessTicketConfiguration());
            modelBuilder.Entity<AfipAccessTicket>().ToTable("FiyiPOS.Common.AfipAccessTicket");
            modelBuilder.ApplyConfiguration(new AfipLogConfiguration());
            modelBuilder.Entity<AfipLog>().ToTable("FiyiPOS.Common.AfipLog");

            //FiyiPOS — Catalogo (variantes con productos y stock: Físico, Online, Restó)
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.Entity<Category>().ToTable("FiyiPOS.Catalogo.Category");
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.Entity<Unit>().ToTable("FiyiPOS.Catalogo.Unit");
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.Entity<Product>().ToTable("FiyiPOS.Catalogo.Product");
            modelBuilder.ApplyConfiguration(new PromotionConfiguration());
            modelBuilder.Entity<Promotion>().ToTable("FiyiPOS.Catalogo.Promotion");
            modelBuilder.ApplyConfiguration(new PromotionDetailConfiguration());
            modelBuilder.Entity<PromotionDetail>().ToTable("FiyiPOS.Catalogo.PromotionDetail");
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());
            modelBuilder.Entity<Supplier>().ToTable("FiyiPOS.Catalogo.Supplier");
            modelBuilder.ApplyConfiguration(new PurchaseConfiguration());
            modelBuilder.Entity<Purchase>().ToTable("FiyiPOS.Catalogo.Purchase");
            modelBuilder.ApplyConfiguration(new PurchaseDetailConfiguration());
            modelBuilder.Entity<PurchaseDetail>().ToTable("FiyiPOS.Catalogo.PurchaseDetail");
            modelBuilder.ApplyConfiguration(new InventoryMovementConfiguration());
            modelBuilder.Entity<InventoryMovement>().ToTable("FiyiPOS.Catalogo.InventoryMovement");
            modelBuilder.ApplyConfiguration(new InternalConsumptionConfiguration());
            modelBuilder.Entity<InternalConsumption>().ToTable("FiyiPOS.Catalogo.InternalConsumption");
            modelBuilder.ApplyConfiguration(new InternalConsumptionDetailConfiguration());
            modelBuilder.Entity<InternalConsumptionDetail>().ToTable("FiyiPOS.Catalogo.InternalConsumptionDetail");

            //FiyiPOS — PuntoDeVenta (flujos exclusivos del mostrador físico)
            modelBuilder.ApplyConfiguration(new CreditConfiguration());
            modelBuilder.Entity<Credit>().ToTable("FiyiPOS.PuntoDeVenta.Credit");
            modelBuilder.ApplyConfiguration(new CreditPaymentConfiguration());
            modelBuilder.Entity<CreditPayment>().ToTable("FiyiPOS.PuntoDeVenta.CreditPayment");
            modelBuilder.ApplyConfiguration(new ReturnSaleConfiguration());
            modelBuilder.Entity<ReturnSale>().ToTable("FiyiPOS.PuntoDeVenta.ReturnSale");
            modelBuilder.ApplyConfiguration(new ReturnSaleDetailConfiguration());
            modelBuilder.Entity<ReturnSaleDetail>().ToTable("FiyiPOS.PuntoDeVenta.ReturnSaleDetail");
        }
    }
}
