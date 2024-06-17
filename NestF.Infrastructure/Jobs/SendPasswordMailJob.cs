using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Domain.Entities;
using NestF.Infrastructure.Constants;
using NestF.Infrastructure.Utils;
using Quartz;

namespace NestF.Infrastructure.Jobs;

public class SendPasswordMailJob : IJob
{
    private readonly IUnitOfWork _uow;
    private readonly EmailConfig _mailConfig;

    public SendPasswordMailJob(IUnitOfWork uow, EmailConfig mailConfig)
    {
        _uow = uow;
        _mailConfig = mailConfig;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var accountId = context.MergedJobDataMap.GetInt(BackgroundConstants.ACCOUNT_ID_KEY);
        var account = await _uow.GetRepo<Account>().GetByIdAsync(accountId);
        if (account == null || account.Email == null) return;
        const string password = "123123123";
        account.PasswordHash = password.Hash();
        if (await _uow.SaveChangesAsync())
        {
            var mailBody = GeneratePasswordMailBody(account.Name, password);
            Email email = new([account.Email], "Tài khoản đăng nhập", mailBody);
            await SendMailAsync(email, new CancellationToken());
        }
    }

    private string GeneratePasswordMailBody(string name, string password)
    {
        var body = $"Mật khẩu của bạn là {password}";
        // string templatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        //     "password.html");
        // var lines = File.ReadLines(templatePath);
        // foreach (var line in lines) body += line;
        // body = body.Replace("{Name}", name);
        // body = body.Replace("{ProviderClientUrl}", config["ProviderClientUrl"]);
        // body = body.Replace("{Password}", password);
        return body;
    }

    private async Task SendMailAsync(Email mailData, CancellationToken cancellationToken)
    {
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_mailConfig.DisplayName, mailData.From ?? _mailConfig.From));
        mail.Sender = new MailboxAddress(mailData.DisplayName ?? _mailConfig.DisplayName,
            mailData.From ?? _mailConfig.From);
        foreach (var mailAddress in mailData.To)
            mail.To.Add(MailboxAddress.Parse(mailAddress));
        if (!string.IsNullOrEmpty(mailData.ReplyTo))
            mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));
        foreach (var mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
            mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
        foreach (var mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
            mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
        var body = new BodyBuilder();
        mail.Subject = mailData.Subject;
        body.HtmlBody = mailData.Body;
        mail.Body = body.ToMessageBody();
        using var smtp = new SmtpClient();
        if (_mailConfig.UseSsl)
            await smtp.ConnectAsync(_mailConfig.Host, _mailConfig.Port, SecureSocketOptions.SslOnConnect,
                cancellationToken);
        else if (_mailConfig.UseStartTls)
            await smtp.ConnectAsync(_mailConfig.Host, _mailConfig.Port, SecureSocketOptions.StartTls,
                cancellationToken);
        await smtp.AuthenticateAsync(_mailConfig.UserName, _mailConfig.Password, cancellationToken);
        await smtp.SendAsync(mail, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
