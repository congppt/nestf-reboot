namespace NestF.Infrastructure.Constants;

public class BackgroundConstants
{
    public const string RETRY_COUNT_KEY = "retries";
    public const int MAX_RETRY_COUNT = 3;
    public const int RETRY_SECOND_MULTYPLY = 15;
    //job data map keys
    public const string ACCOUNT_ID_KEY = "account_id";
    public const string PRODUCT_ID_KEY = "product_id";
    //job groups
    public const string ACCOUNT_GRP = "ACCOUNT";
    public const string PRODUCT_GRP = "PRODUCT";

}