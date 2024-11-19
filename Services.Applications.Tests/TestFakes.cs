﻿using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Tests;

public static class TestFakes 
{

    public static DateTime Today => new DateTime(2024, 11, 19);
    public static Application Application(int yearOfBirth, int monthOfBirth, int dayOfBirth)
        => new Application
        {
            Applicant = new User { DateOfBirth = new DateOnly(yearOfBirth, monthOfBirth, dayOfBirth) }
        };
    public static Application Application()
        => new Application
        {
            Applicant = new User { DateOfBirth = new DateOnly(2000, 1, 1) }
        };

    public static CreateInvestorResponse CreateInvestorResponse() =>
        new CreateInvestorResponse
        {
            InvestorId = string.Empty,
            AccountId = string.Empty
        };
}
