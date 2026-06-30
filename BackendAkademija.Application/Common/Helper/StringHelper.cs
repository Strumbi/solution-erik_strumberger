namespace BackendAkademija.Application.Helper;

public static class StringHelper
{
    public static string Truncate(string value, int maxLength) => value.Length <= maxLength ? value : value[..maxLength] + "...";
}