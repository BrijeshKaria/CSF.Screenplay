﻿//
// JsonHttpRequestFactory.cs
//
// Author:
//       Craig Fowler <craig@csf-dev.com>
//
// Copyright (c) 2018 Craig Fowler
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CSF.Screenplay.WebApis.Services
{
  class JsonHttpRequestFactory
  {
    readonly ICreatesRequestBodies requestBodyFactory;
    readonly ICreatesHttpRequests requestFactory;

    internal HttpRequestMessage GetRequest(IEndpoint endpoint, object payload)
    {
      var requestBody = GetRequestBody(payload);
      var request = requestFactory.CreateRequest(endpoint, requestBody);
      AddAcceptsJsonHeader(request);
      return request;
    }

    HttpContent GetRequestBody(object payload)
    {
      if(payload == null) return null;
      return requestBodyFactory.CreateRequestBody(payload);
    }

    void AddAcceptsJsonHeader(HttpRequestMessage request)
    {
      var jsonMimeType = JsonHttpContentReaderWriter.JsonContentType.MediaType;
      request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonMimeType, 1));
    }

    public JsonHttpRequestFactory()
    {
      requestBodyFactory = new JsonHttpContentReaderWriter();
      requestFactory = new HttpRequestFactory();
    }
  }
}
