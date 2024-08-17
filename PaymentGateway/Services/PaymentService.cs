﻿using Common.Constants;
using Common.Contexts.Models;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Common.Security;
using Common.Utilities;
using Newtonsoft.Json;
using PaymentGateway.Models;
using PaymentGateway.Services.Interfaces;

namespace PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<TransactionTableModel> transactionRepo;
        private readonly string serviceName = "Payment Service";

        public PaymentService(IRepository<TransactionTableModel> transactionRepo)
        {
            this.transactionRepo = transactionRepo;
        }

        public PaymentResponseModel CreditCardPayment(string encryptedString)
        {
            GlobalLoggingHandler.Logging.Info($"{serviceName}[CreditCardPayment] Start method");   

            var transactionRef = Util.GenerateReference(16);

            string payment = Decryptor.DecryptText(encryptedString);

            CreditCardPaymentModel ccModel = new CreditCardPaymentModel();

            try
            {
                ccModel = JsonConvert.DeserializeObject<CreditCardPaymentModel>(payment);
            }
            catch
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[CreditCardPayment] Error converting Json to model");
                throw new GeneralException();
            }
            

            ccModel.TransactionReferenceNo = transactionRef;

            var encryptedModel = Encryptor.EncryptText(JsonConvert.SerializeObject(ccModel));

            //Mock successful payment response

            var result = new PaymentResponseModel()
            {
                CompletedTransaction = true
            };


            var transaction = new TransactionTableModel()
            {
                BookingReferenceNo = ccModel.BookingReferenceNo,
                Name = ccModel.Name,
                PaymentType = (int)PaymentType.CreditCard,
                TransactionReferenceNo = transactionRef,
                TransactionStatus = result.CompletedTransaction
            };

            GlobalLoggingHandler.Logging.Info($"{serviceName}[CreditCardPayment] Insert transaction into database | {transaction}");
            transactionRepo.Add(transaction);
            transactionRepo.SaveChanges();

            GlobalLoggingHandler.Logging.Info($"{serviceName}[CreditCardPayment] End method");
            return result;
        }
    }
}
