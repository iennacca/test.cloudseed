
namespace CloudSeedApp {

    public enum PublicErrorResponseMessages {
        InternalServerError, // This is the default

        // User
        InvalidEmailAddress,
        Login_FailedToLogIn,
        Login_InvalidAccesstoken,
        UserAccountDoesNotExist,
        RegisterUser_UserWithEmailAlreadyExists,
    }

}