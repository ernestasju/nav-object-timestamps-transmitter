namespace nav_object_timestamps_transmitter
{
    partial class Program
    {
	static string timestampDataQuery = @"
SELECT
	[Type],
	[ID],
	[timestamp] = CONVERT(BIGINT, CONVERT(VARBINARY(8), [timestamp]))
FROM
	[Object]
WHERE
	[Type] NOT IN (0, 10, 11) -- TableData|System|FieldNumber
";
    }
}
