using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Core.Services.Interfaces;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ShippingZoneGatewayApiController : MerchelloApiController
    {
        /// <summary>
        /// The ship zone service
        /// </summary>
        private readonly IShipZoneService _shipZoneService;

        /// <summary>
        /// The ship country service.
        /// </summary>
        private readonly IShipZoneCountryService _shipZoneCountryService;

        /// <summary>
        /// The shipping context.
        /// </summary>
        private readonly IShippingContext _shippingContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayApiController"/> class.
        /// </summary>
        public ShippingZoneGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ShippingZoneGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shippingContext = MerchelloContext.Gateways.Shipping;

            _shipZoneCountryService = ((ServiceContext)MerchelloContext.Services).ShipZoneCountryService;
            _shipZoneService = ((ServiceContext)MerchelloContext.Services).ShipZoneService;
        }


        #region Shipping Zones

        /// <summary>
        /// Creates a ship zone
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/NewShipzone?catalogKey={guid}&amp;zoneName={string}
        /// </summary>
        /// <param name="catalogKey">
        /// CatalogKey Guid
        /// </param>
        /// <param name="zoneName">
        /// zone name string
        /// </param>
        /// <returns>
        /// The <see cref="ShipZoneDisplay"/>.
        /// </returns>        
        [AcceptVerbs("GET", "POST")]
        public ShipZoneDisplay NewShipzone(Guid catalogKey, string zoneName)
        {
            ShipZone newShipzone = null;

            try
            {
                var attempt = ((ShipZoneService)_shipZoneService).CreateShipZoneWithKey(catalogKey, zoneName);
                if (attempt.Success)
                {
                    newShipzone = attempt.Result as ShipZone;
                }
                else
                {
                    throw attempt.Exception;
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            return newShipzone.ToShipZoneDisplay();
        }

        /// <summary>
        /// Gets all ship zones.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        public IEnumerable<ShipZoneDisplay> GetAllShipZones(Guid id)
        {
            var zones = this._shipZoneService.GetShipZonesByCatalogKey(id);
            if (zones == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return zones.Select(zone => zone.ToShipZoneDisplay());
        }

        /// <summary>
        /// Gets the ship zone.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        public ShipZoneDisplay GetShipZone(Guid id)
        {
            var shipZone = this._shipZoneService.GetByKey(id);

            if (shipZone == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return shipZone.ToShipZoneDisplay();
        }

        /// <summary>
        /// Deletes the ship zone.
        /// </summary>
        /// <param name="id">The ShipZone key.</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET")]
        public HttpResponseMessage DeleteShipZone(Guid id)
        {
            var shipZoneToDelete = _shipZoneService.GetByKey(id);
            if (shipZoneToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _shipZoneService.Delete(shipZoneToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion

    }
}
