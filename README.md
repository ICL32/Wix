Thoughts:

I'd typically go with an int for ID but it seems from the task a string is specified. I'll set it as a key and set a string length as well.

Usually timestamp isnt stored as an int but as a long within .NET as APIs with Unix time return. 
But will still keep it as an integer as the task says.

Example:
https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-8.0

It seems in the post example all the fields are entered in and
no calculations are done for time as it's a user input.

# Validations:

## Fixed size types:

Integers:

- Number isnt negative
- Number is the right size

Strings:

- String limits are set for safety
- Strings cant be null