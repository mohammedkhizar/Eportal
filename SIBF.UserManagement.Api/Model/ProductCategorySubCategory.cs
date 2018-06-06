namespace SIBF.UserManagement.Api
{
    public enum ProductCategorySubCategory
    {
        //
        // Summary:
        //     The {category, subcategory or product} was successfully created.
        Success = 0,
        //
        // Summary:
        //     The name of {category, subcategory or product} was not found in the database.
        InvalidUserName = 1,
        //
        // Summary:
        //     The {category, subcategory or product} is not formatted correctly.
        InvalidPassword = 2,
        //
        // Summary:
        //     The {category, subcategory or product} address is not formatted correctly.
        InvalidEmail = 3,
        //
        // Summary:
        //     The name {category, subcategory or product} already exists in the database for the application.
        DuplicateName = 4,
        //
        // Summary:
        //     The e-mail address already exists in the database for the application.
        //DuplicateEmail = 5,
        //
        // Summary:
        //     The {category, subcategory or product} was not created, for a reason defined by the provider.
        UserRejected = 5
    }
}
