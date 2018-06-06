using System;

namespace SIBF.UserManagement.Api
{
    public class DbException : Exception
    {
        public DbException(){}
        public DbException(string message) : base(message){}
        public DbException(string message, Exception inner): base(message, inner){}
    }

    public class DuplicateRoleNameException : Exception
    {
        public DuplicateRoleNameException() { }
        public DuplicateRoleNameException(string message) : base(message) { }
        public DuplicateRoleNameException(string message, Exception inner) : base(message, inner) { }
    }

    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() { }
        public RoleNotFoundException(string message) : base(message) { }
        public RoleNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class UsersAvialbleForRoleException : Exception
    {
        public UsersAvialbleForRoleException() { }
        public UsersAvialbleForRoleException(string message) : base(message) { }
        public UsersAvialbleForRoleException(string message, Exception inner) : base(message, inner) { }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() { }
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() { }
        public InvalidPasswordException(string message) : base(message) { }
        public InvalidPasswordException(string message, Exception inner) : base(message, inner) { }
    }

    public class DuplicateColumnException : Exception
    {
        public DuplicateColumnException() { }
        public DuplicateColumnException(string message) : base(message) { }
        public DuplicateColumnException(string message, Exception inner) : base(message, inner) { }

    }

}
