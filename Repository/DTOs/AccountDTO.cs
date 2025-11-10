namespace Repository.DTOs
{
    public class AccountDTO
    {
        public class ProfileResponse
        {
            public int AccountId { get; set; }
            public string AccountEmail { get; set; } = string.Empty;
            public string AccountName { get; set; } = string.Empty;
            public int AccountRole { get; set; }
        }

        public class AccountResponse
        {
            public int AccountId { get; set; }
            public string AccountName { get; set; } = string.Empty;
            public string AccountEmail { get; set; } = string.Empty;
            public int AccountRole { get; set; }
            public int NewsCount { get; set; }
        }

        public class CreateAccountRequest
        {
            public string AccountName { get; set; } = string.Empty;
            public string AccountEmail { get; set; } = string.Empty;
            public string AccountPassword { get; set; } = string.Empty;
            public int AccountRole { get; set; }
        }

        public class UpdateAccountRequest
        {
            public string AccountName { get; set; } = string.Empty;
            public string AccountEmail { get; set; } = string.Empty;
            public int AccountRole { get; set; }
        }

        public class ChangePasswordRequest
        {
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}