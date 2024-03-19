Assumptions:
- ID:


We wont ever assign a negative integer as an ID

- Timestamp:

Assuming we're using a unix timestamp it's better if we store it as a long within .NET environments because a lot of methods when returning Unix timestamps within .NET will return it as a long.
Example:
https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-8.0

