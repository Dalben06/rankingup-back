using System;
using System.Data;
using Dapper;

namespace RankingUp.WebApp.API.Setup;

public sealed class TimeOnlyHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value.ToTimeSpan();
    }

    public override TimeOnly Parse(object value)
        => value switch
        {
            TimeSpan ts => TimeOnly.FromTimeSpan(ts),
            DateTime dt => TimeOnly.FromDateTime(dt),
            TimeOnly to => to,
            string s => TimeOnly.TryParse(s, out var result)
                ? result
                : throw new DataException($"Cannot parse '{s}' to TimeOnly"),
            _ => throw new DataException($"Cannot convert {value.GetType()} to TimeOnly")
        };
}

public sealed class NullableTimeOnlyHandler : SqlMapper.TypeHandler<TimeOnly?>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly? value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value?.ToTimeSpan() ?? (object)DBNull.Value;
    }

    public override TimeOnly? Parse(object value)
        => value switch
        {
            null or DBNull => null,
            TimeSpan ts => TimeOnly.FromTimeSpan(ts),
            DateTime dt => TimeOnly.FromDateTime(dt),
            TimeOnly to => to,
            string s => TimeOnly.Parse(s),
            _ => throw new DataException($"Cannot convert {value.GetType()} to TimeOnly?")
        };
}
