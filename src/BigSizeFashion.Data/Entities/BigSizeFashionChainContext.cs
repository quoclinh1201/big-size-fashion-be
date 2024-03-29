﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class BigSizeFashionChainContext : DbContext
    {
        public BigSizeFashionChainContext()
        {
        }

        public BigSizeFashionChainContext(DbContextOptions<BigSizeFashionChainContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Colour> Colours { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCart> CustomerCarts { get; set; }
        public virtual DbSet<DeliveryNote> DeliveryNotes { get; set; }
        public virtual DbSet<DeliveryNoteDetail> DeliveryNoteDetails { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<InventoryNote> InventoryNotes { get; set; }
        public virtual DbSet<InventoryNoteDetail> InventoryNoteDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<PromotionDetail> PromotionDetails { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreWarehouse> StoreWarehouses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<staff> staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Account__DD70126471B7DB91");

                entity.ToTable("Account");

                entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572C1DC7D81")
                    .IsUnique();

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Account__role_id__29572725");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.ReceiveAddress)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("receive_address");

                entity.Property(e => e.ReceiverName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("receiver_name");

                entity.Property(e => e.ReceiverPhone)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("receiver_phone");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Address__custome__31EC6D26");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("category_name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Colour>(entity =>
            {
                entity.ToTable("Colour");

                entity.Property(e => e.ColourId).HasColumnName("colour_id");

                entity.Property(e => e.ColourCode)
                    .IsRequired()
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("colour_code");

                entity.Property(e => e.ColourName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("colour_name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Customer__DD7012649240980A");

                entity.ToTable("Customer");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("avatar_url");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(50)
                    .HasColumnName("fullname");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__uid__2F10007B");
            });

            modelBuilder.Entity<CustomerCart>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.ProductDetailId })
                    .HasName("PK__Customer__E0BE81565D99B320");

                entity.ToTable("CustomerCart");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCarts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerC__custo__44FF419A");

                entity.HasOne(d => d.ProductDetail)
                    .WithMany(p => p.CustomerCarts)
                    .HasForeignKey(d => d.ProductDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerC__produ__45F365D3");
            });

            modelBuilder.Entity<DeliveryNote>(entity =>
            {
                entity.ToTable("DeliveryNote");

                entity.Property(e => e.DeliveryNoteId).HasColumnName("delivery_note_id");

                entity.Property(e => e.ApprovalDate)
                    .HasColumnType("datetime")
                    .HasColumnName("approval_date");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.DeliveryNoteName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("delivery_note_name");

                entity.Property(e => e.FromStore).HasColumnName("from_store");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ToStore).HasColumnName("to_store");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.HasOne(d => d.FromStoreNavigation)
                    .WithMany(p => p.DeliveryNoteFromStoreNavigations)
                    .HasForeignKey(d => d.FromStore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeliveryN__from___534D60F1");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.DeliveryNotes)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeliveryN__staff__52593CB8");

                entity.HasOne(d => d.ToStoreNavigation)
                    .WithMany(p => p.DeliveryNoteToStoreNavigations)
                    .HasForeignKey(d => d.ToStore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeliveryN__to_st__5441852A");
            });

            modelBuilder.Entity<DeliveryNoteDetail>(entity =>
            {
                entity.HasKey(e => new { e.DeliveryNoteId, e.ProductDetailId })
                    .HasName("PK__Delivery__154BEB5524A9284E");

                entity.ToTable("DeliveryNoteDetail");

                entity.Property(e => e.DeliveryNoteId).HasColumnName("delivery_note_id");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.DeliveryNote)
                    .WithMany(p => p.DeliveryNoteDetails)
                    .HasForeignKey(d => d.DeliveryNoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeliveryN__deliv__571DF1D5");

                entity.HasOne(d => d.ProductDetail)
                    .WithMany(p => p.DeliveryNoteDetails)
                    .HasForeignKey(d => d.ProductDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeliveryN__produ__5812160E");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Feedback__custom__34C8D9D1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Feedback__produc__35BCFE0A");
            });

            modelBuilder.Entity<InventoryNote>(entity =>
            {
                entity.ToTable("InventoryNote");

                entity.Property(e => e.InventoryNoteId).HasColumnName("inventory_note_id");

                entity.Property(e => e.AdjustedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("adjusted_date");

                entity.Property(e => e.FromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("from_date");

                entity.Property(e => e.InventoryNoteName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("inventory_note_name");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.ToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("to_date");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.InventoryNotes)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__staff__5BE2A6F2");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.InventoryNotes)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__store__5AEE82B9");
            });

            modelBuilder.Entity<InventoryNoteDetail>(entity =>
            {
                entity.HasKey(e => new { e.InventoryNoteId, e.ProductDetailId })
                    .HasName("PK__Inventor__B9BC3F0B86DA9D36");

                entity.ToTable("InventoryNoteDetail");

                entity.Property(e => e.InventoryNoteId).HasColumnName("inventory_note_id");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.BeginningQuantity).HasColumnName("beginning_quantity");

                entity.Property(e => e.EndingQuantity).HasColumnName("ending_quantity");

                entity.Property(e => e.EndingQuantityAfterAdjusted).HasColumnName("ending_quantity_after_adjusted");

                entity.HasOne(d => d.InventoryNote)
                    .WithMany(p => p.InventoryNoteDetails)
                    .HasForeignKey(d => d.InventoryNoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__inven__5EBF139D");

                entity.HasOne(d => d.ProductDetail)
                    .WithMany(p => p.InventoryNoteDetails)
                    .HasForeignKey(d => d.ProductDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__produ__5FB337D6");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("message");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificat__accou__2C3393D0");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ApprovalDate)
                    .HasColumnType("datetime")
                    .HasColumnName("approval_date");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.DeliveryAddress).HasColumnName("delivery_address");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("delivery_date");

                entity.Property(e => e.OrderType).HasColumnName("order_type");

                entity.Property(e => e.PackagedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("packaged_date");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");

                entity.Property(e => e.ReceivedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("received_date");

                entity.Property(e => e.RejectedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("rejected_date");

                entity.Property(e => e.ShippingFee)
                    .HasColumnType("money")
                    .HasColumnName("shipping_fee");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.Property(e => e.TotalPriceAfterDiscount)
                    .HasColumnType("money")
                    .HasColumnName("total_price_after_discount");

                entity.Property(e => e.ZpTransId)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("zp_trans_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__customer___48CFD27E");

                entity.HasOne(d => d.DeliveryAddressNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryAddress)
                    .HasConstraintName("FK__Order__delivery___49C3F6B7");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__Order__staff_id__4BAC3F29");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__store_id__4AB81AF0");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductDetailId })
                    .HasName("PK__OrderDet__6B8228FAC608F474");

                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.DiscountPrice)
                    .HasColumnType("money")
                    .HasColumnName("discount_price");

                entity.Property(e => e.DiscountPricePerOne)
                    .HasColumnType("money")
                    .HasColumnName("discount_price_per_one");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.PricePerOne)
                    .HasColumnType("money")
                    .HasColumnName("price_per_one");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__order__4E88ABD4");

                entity.HasOne(d => d.ProductDetail)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__produ__4F7CD00D");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("brand");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("product_name");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__categor__164452B1");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.ToTable("ProductDetail");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.ColourId).HasColumnName("colour_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.SizeId).HasColumnName("size_id");

                entity.HasOne(d => d.Colour)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ColourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductDe__colou__1A14E395");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductDe__produ__1920BF5C");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.SizeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductDe__size___1B0907CE");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");

                entity.Property(e => e.ProductImageId).HasColumnName("product_image_id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("image_url");

                entity.Property(e => e.IsMainImage).HasColumnName("is_main_image");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__produ__239E4DCF");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("Promotion");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.ApplyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("apply_date");

                entity.Property(e => e.ExpiredDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expired_date");

                entity.Property(e => e.PromotionName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("promotion_name");

                entity.Property(e => e.PromotionValue).HasColumnName("promotion_value");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<PromotionDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.PromotionId })
                    .HasName("PK__Promotio__E5C9E8A3F98D7461");

                entity.ToTable("PromotionDetail");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PromotionDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Promotion__produ__1FCDBCEB");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionDetails)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Promotion__promo__20C1E124");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("Size");

                entity.Property(e => e.SizeId).HasColumnName("size_id");

                entity.Property(e => e.SizeName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("size_name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.IsMainWarehouse).HasColumnName("is_main_warehouse");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreAddress)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("store_address");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("store_name");

                entity.Property(e => e.StorePhone)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("store_phone");
            });

            modelBuilder.Entity<StoreWarehouse>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.ProductDetailId })
                    .HasName("PK__StoreWar__8F29E9DF36073A54");

                entity.ToTable("StoreWarehouse");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.ProductDetailId).HasColumnName("product_detail_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.ProductDetail)
                    .WithMany(p => p.StoreWarehouses)
                    .HasForeignKey(d => d.ProductDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StoreWare__produ__3B75D760");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreWarehouses)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StoreWare__store__3A81B327");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__DD701264BA1BEC6D");

                entity.ToTable("User");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(50)
                    .HasColumnName("fullname");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__uid__4222D4EF");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Staff__DD701264A8ACD52D");

                entity.ToTable("Staff");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("avatar_url");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(50)
                    .HasColumnName("fullname");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.staff)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Staff__store_id__3F466844");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.staff)
                    .HasForeignKey<staff>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Staff__uid__3E52440B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
