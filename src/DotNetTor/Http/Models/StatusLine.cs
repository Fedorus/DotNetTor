﻿using System;
using static DotNetTor.Http.Constants;
using System.Net;
using DotNetEssentials.Logging;

namespace DotNetTor.Http.Models
{
	public class StatusLine : StartLine
	{
		public HttpStatusCode StatusCode { get; private set; }

		public StatusLine(HttpProtocol protocol, HttpStatusCode status)
		{
			Protocol = protocol;
			StatusCode = status;

			StartLineString = Protocol.ToString() + SP + (int)StatusCode + SP + StatusCode.ToReasonString() + CRLF;
		}

		public static StatusLine CreateNew(string statusLineString)
		{
			try
			{
				var parts = GetParts(statusLineString);
				var protocolString = parts[0];
				var codeString = parts[1];
				var reason = parts.Count > 2 ? parts[2] : string.Empty;
				var protocol = new HttpProtocol(protocolString);
				var code = int.Parse(codeString);
				if (!HttpStatusCodeHelper.IsValidCode(code))
				{
					throw new NotSupportedException($"Invalid HTTP status code: {code}.");
				}

				var statusCode = (HttpStatusCode)code;

				// https://tools.ietf.org/html/rfc7230#section-3.1.2
				// The reason-phrase element exists for the sole purpose of providing a
				// textual description associated with the numeric status code, mostly
				// out of deference to earlier Internet application protocols that were
				// more frequently used with interactive text clients.A client SHOULD
				// ignore the reason - phrase content.

				return new StatusLine(protocol, statusCode);
			}
			catch (Exception ex)
			{
				Logger.LogDebug<StatusLine>(ex);
				throw new NotSupportedException($"Invalid {nameof(StatusLine)}: {statusLineString}.", ex);
			}
		}
	}
}
