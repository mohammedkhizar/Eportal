namespace SIBF.UserManagement.Api
{
    public enum MembershipCreateStatus
    {
        //
        // Summary:
        //     The user was successfully created.
        Success = 0,
        //
        // Summary:
        //     The user name was not found in the database.
        InvalidUserName = 1,
        //
        // Summary:
        //     The password is not formatted correctly.
        InvalidPassword = 2,       
        //
        // Summary:
        //     The e-mail address is not formatted correctly.
        InvalidEmail = 3,
        //
        // Summary:
        //     The user name already exists in the database for the application.
        DuplicateUserName = 4,
        //
        // Summary:
        //     The e-mail address already exists in the database for the application.
        DuplicateEmail = 5,
        //
        // Summary:
        //     The user was not created, for a reason defined by the provider.
        UserRejected = 6
    }
}
