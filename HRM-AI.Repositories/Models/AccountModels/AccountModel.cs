using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Models.AccountRoleModels;

namespace HRM_AI.Repositories.Models.AccountModels
{
    public class AccountModel : BaseEntity
    {
        // Required information
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        // Personal information
        public Gender? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        // public string? StoreAddress { get; set; }
        public string? StoreDescription { get; set; }
        public string? Image { get; set; }
        public string? Banner { get; set; }
        public double? SuccessDeliveryRate { get; set; }

        // Status
        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        //public AccountStatus Status { get; set; }

        // Wallet information
        public decimal Balance { get; set; } // Lấy số dư ví
        public decimal TotalAmountPaid { get; set; } // Tổng tiền đã chi trả

        // Order information
        public int OrderCount { get; set; }
        public int OrderArtisanCount { get; set; }
        public int ServiceCount { get; set; }

        // Relationship
        // public List<Role> Roles { get; set; } = null!;
        // public List<string> RoleNames { get; set; } = null!;
        public List<AccountRoleModel> AccountRoles { get; set; } = null!;
    }
}
