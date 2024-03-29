﻿using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Business.BusinessAspects.Autofac;
using Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;
        private ICarService _carService;
        private IHttpContextAccessor _httpContextAccessor;
        private ICustomerService _customerService;

        public RentalManager(IRentalDal rentalDal, IHttpContextAccessor httpContextAccessor, ICarService carService, ICustomerService customerService)
        {
            _rentalDal = rentalDal;
            _httpContextAccessor = httpContextAccessor;
            _carService = carService;
            _customerService = customerService;

        }

        [Authentication]
        public IResult Add(RentalForAddDto rental)
        {
            int authUserId = _httpContextAccessor.HttpContext.User.GetAuthenticatedUserId();
            var customer = _customerService.GetByUserId(authUserId);
            if (customer.Success == false)
            {
                return new ErrorResult();
            }
            var result = BusinessRules.Run(IsCarReturned(rental.CarId),IsCarExists(rental.CarId),IsFindexEnough(rental.CarId,customer.Data.Id));
            if (result!=null)
            {
                return result;
            }
            var carToRent = _carService.GetById(rental.CarId);
            Rental rentalToAdd = new Rental();
            rentalToAdd.RentDate = DateTime.Now;
            rentalToAdd.CarId = rental.CarId;
            rentalToAdd.CustomerId = customer.Data.Id;
            rentalToAdd.RentDays = rental.RentDays;
            rentalToAdd.TotalPrice = (decimal)rental.RentDays * carToRent.Data.DailyPrice;
            _rentalDal.Add(rentalToAdd);
            return new SuccessResult(Messages.RentalAdded);
            
        }

        public IResult Delete(Rental rental)
        {
            _rentalDal.Delete(rental);
            return new SuccessResult(Messages.RentalDeleted);
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
        }

        public IResult Update(Rental rental)
        {
            _rentalDal.Update(rental);
            return new SuccessResult(Messages.RentalUpdated);
        }

        public IDataResult<Rental> GetById(int id)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r=>r.Id==id));
        }

        public IDataResult<List<RentalDetailDto>> GetRentalDetails()
        {
            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalDetails());
        }

        private IResult IsCarReturned(int id)
        {
            var result = _rentalDal.Get(c => c.CarId == id && c.ReturnDate == null);
            if (result != null)
            {
                return new ErrorResult(Messages.CarBusy);
            }
            return new SuccessResult();
        }

        private IResult IsCarExists(int id)
        {
            var result = _carService.GetById(id);
            if (result.Data == null)
            {
                return new ErrorResult(Messages.CarNotExists);
            }
            return new SuccessResult();
        }

        private IResult IsFindexEnough(int carId,int customerId)
        {
            int carFindex = _carService.GetCarFindeks(carId).Data;
            int userFindex = _customerService.GetCustomerFindex(customerId).Data;

            if (userFindex>=carFindex)
            {
                return new SuccessResult();
            }
            return new ErrorResult();
        }

    }
}
