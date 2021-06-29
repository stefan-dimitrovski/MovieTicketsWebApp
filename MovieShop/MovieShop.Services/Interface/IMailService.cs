using MovieShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Services.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
