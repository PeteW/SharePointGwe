<%@ Page Language="C#" Inherits="System.Web.UI.Page" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.Utilities" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<% Response.ContentType = "text/xml"; %>
<wsdl:definitions xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:tm="http://microsoft.com/wsdl/mime/textMatching/" xmlns:soapenc="http://schemas.xmlsoap.org/soap/encoding/" xmlns:mime="http://schemas.xmlsoap.org/wsdl/mime/" xmlns:tns="http://tempuri.org/" xmlns:s="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://schemas.xmlsoap.org/wsdl/soap12/" xmlns:http="http://schemas.xmlsoap.org/wsdl/http/" targetNamespace="http://tempuri.org/" xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/">
  <wsdl:types>
    <s:schema elementFormDefault="qualified" targetNamespace="http://tempuri.org/">
      <s:element name="GetFormHistory">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="historyListName" type="s:string" />
            <s:element minOccurs="0" maxOccurs="1" name="formTitle" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetFormHistoryResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetFormHistoryResult" type="tns:ArrayOfFormHistoryItem" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="ArrayOfFormHistoryItem">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="FormHistoryItem" nillable="true" type="tns:FormHistoryItem" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="FormHistoryItem">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Date" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetManager">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="domainName" type="s:string" />
            <s:element minOccurs="0" maxOccurs="1" name="userName" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetManagerResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetManagerResult" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetUserFullIdentity">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="request" type="tns:GetUserFullIdentityRequest" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetUserFullIdentityRequest">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceRequestModel">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="DomainName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="UserName" type="s:string" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="BaseWebServiceRequestModel">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Id" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetUserFullIdentityResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetUserFullIdentityResult" type="tns:GetUserFullIdentityResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetUserFullIdentityResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceResponseModel">
            <s:sequence>
              <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
              <s:element minOccurs="0" maxOccurs="1" name="FullName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="UserName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="Email" type="s:string" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="BaseWebServiceResponseModel">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="ExecutionOutcome" type="tns:ExecutionOutcome" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ExecutionOutcome">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="IsSuccessful" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Message" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetUserEmail">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="domainName" type="s:string" />
            <s:element minOccurs="0" maxOccurs="1" name="userName" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetUserEmailResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetUserEmailResult" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="IsUserMemberOfSharePointGroup">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="domainName" type="s:string" />
            <s:element minOccurs="0" maxOccurs="1" name="userName" type="s:string" />
            <s:element minOccurs="0" maxOccurs="1" name="sharePointGroupName" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="IsUserMemberOfSharePointGroupResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="IsUserMemberOfSharePointGroupResult" type="s:boolean" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="Response1">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="groupName" type="s:string" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="Response1Response">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="Response1Result" type="tns:GetSharePointUsersByGroupResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetSharePointUsersByGroupResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceResponseModel">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="Items" type="tns:ArrayOfGetSharePointUsersByGroupResponseItem" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfGetSharePointUsersByGroupResponseItem">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="GetSharePointUsersByGroupResponseItem" nillable="true" type="tns:GetSharePointUsersByGroupResponseItem" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="GetSharePointUsersByGroupResponseItem">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="FullName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="UserName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Email" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="DeleteNode">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="request" type="tns:DeleteNodeRequest" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="DeleteNodeRequest">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceRequestModel">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="FileName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="ListName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="NodeXpath" type="s:string" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="DeleteNodeResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="DeleteNodeResult" type="tns:DeleteNodeResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="DeleteNodeResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceResponseModel" />
        </s:complexContent>
      </s:complexType>
      <s:element name="AppendXml">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="request" type="tns:AppendXmlRequest" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="AppendXmlRequest">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceRequestModel">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="FileName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="ListName" type="s:string" />
              <s:element minOccurs="0" maxOccurs="1" name="Xml" type="s:string" />
              <s:element minOccurs="1" maxOccurs="1" name="Prepend" type="s:boolean" />
              <s:element minOccurs="0" maxOccurs="1" name="ParentNodeXpath" type="s:string" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="AppendXmlResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="AppendXmlResult" type="tns:AppendXmlResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="AppendXmlResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseWebServiceResponseModel" />
        </s:complexContent>
      </s:complexType>
    </s:schema>
  </wsdl:types>
  <wsdl:message name="GetFormHistorySoapIn">
    <wsdl:part name="parameters" element="tns:GetFormHistory" />
  </wsdl:message>
  <wsdl:message name="GetFormHistorySoapOut">
    <wsdl:part name="parameters" element="tns:GetFormHistoryResponse" />
  </wsdl:message>
  <wsdl:message name="GetManagerSoapIn">
    <wsdl:part name="parameters" element="tns:GetManager" />
  </wsdl:message>
  <wsdl:message name="GetManagerSoapOut">
    <wsdl:part name="parameters" element="tns:GetManagerResponse" />
  </wsdl:message>
  <wsdl:message name="GetUserFullIdentitySoapIn">
    <wsdl:part name="parameters" element="tns:GetUserFullIdentity" />
  </wsdl:message>
  <wsdl:message name="GetUserFullIdentitySoapOut">
    <wsdl:part name="parameters" element="tns:GetUserFullIdentityResponse" />
  </wsdl:message>
  <wsdl:message name="GetUserEmailSoapIn">
    <wsdl:part name="parameters" element="tns:GetUserEmail" />
  </wsdl:message>
  <wsdl:message name="GetUserEmailSoapOut">
    <wsdl:part name="parameters" element="tns:GetUserEmailResponse" />
  </wsdl:message>
  <wsdl:message name="IsUserMemberOfSharePointGroupSoapIn">
    <wsdl:part name="parameters" element="tns:IsUserMemberOfSharePointGroup" />
  </wsdl:message>
  <wsdl:message name="IsUserMemberOfSharePointGroupSoapOut">
    <wsdl:part name="parameters" element="tns:IsUserMemberOfSharePointGroupResponse" />
  </wsdl:message>
  <wsdl:message name="Response1SoapIn">
    <wsdl:part name="parameters" element="tns:Response1" />
  </wsdl:message>
  <wsdl:message name="Response1SoapOut">
    <wsdl:part name="parameters" element="tns:Response1Response" />
  </wsdl:message>
  <wsdl:message name="DeleteNodeSoapIn">
    <wsdl:part name="parameters" element="tns:DeleteNode" />
  </wsdl:message>
  <wsdl:message name="DeleteNodeSoapOut">
    <wsdl:part name="parameters" element="tns:DeleteNodeResponse" />
  </wsdl:message>
  <wsdl:message name="AppendXmlSoapIn">
    <wsdl:part name="parameters" element="tns:AppendXml" />
  </wsdl:message>
  <wsdl:message name="AppendXmlSoapOut">
    <wsdl:part name="parameters" element="tns:AppendXmlResponse" />
  </wsdl:message>
  <wsdl:portType name="GweWebServicesSoap">
    <wsdl:operation name="GetFormHistory">
      <wsdl:input message="tns:GetFormHistorySoapIn" />
      <wsdl:output message="tns:GetFormHistorySoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetManager">
      <wsdl:input message="tns:GetManagerSoapIn" />
      <wsdl:output message="tns:GetManagerSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetUserFullIdentity">
      <wsdl:input message="tns:GetUserFullIdentitySoapIn" />
      <wsdl:output message="tns:GetUserFullIdentitySoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetUserEmail">
      <wsdl:input message="tns:GetUserEmailSoapIn" />
      <wsdl:output message="tns:GetUserEmailSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="IsUserMemberOfSharePointGroup">
      <wsdl:input message="tns:IsUserMemberOfSharePointGroupSoapIn" />
      <wsdl:output message="tns:IsUserMemberOfSharePointGroupSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSharePointUsersByGroup">
      <wsdl:input name="Response1" message="tns:Response1SoapIn" />
      <wsdl:output name="Response1" message="tns:Response1SoapOut" />
    </wsdl:operation>
    <wsdl:operation name="DeleteNode">
      <wsdl:input message="tns:DeleteNodeSoapIn" />
      <wsdl:output message="tns:DeleteNodeSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="AppendXml">
      <wsdl:input message="tns:AppendXmlSoapIn" />
      <wsdl:output message="tns:AppendXmlSoapOut" />
    </wsdl:operation>
  </wsdl:portType>
  <wsdl:binding name="GweWebServicesSoap" type="tns:GweWebServicesSoap">
    <soap:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="GetFormHistory">
      <soap:operation soapAction="http://tempuri.org/GetFormHistory" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetManager">
      <soap:operation soapAction="http://tempuri.org/GetManager" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetUserFullIdentity">
      <soap:operation soapAction="http://tempuri.org/GetUserFullIdentity" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetUserEmail">
      <soap:operation soapAction="http://tempuri.org/GetUserEmail" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="IsUserMemberOfSharePointGroup">
      <soap:operation soapAction="http://tempuri.org/IsUserMemberOfSharePointGroup" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSharePointUsersByGroup">
      <soap:operation soapAction="http://tempuri.org/Response1" style="document" />
      <wsdl:input name="Response1">
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output name="Response1">
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="DeleteNode">
      <soap:operation soapAction="http://tempuri.org/DeleteNode" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="AppendXml">
      <soap:operation soapAction="http://tempuri.org/AppendXml" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
  </wsdl:binding>
  <wsdl:binding name="GweWebServicesSoap12" type="tns:GweWebServicesSoap">
    <soap12:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="GetFormHistory">
      <soap12:operation soapAction="http://tempuri.org/GetFormHistory" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetManager">
      <soap12:operation soapAction="http://tempuri.org/GetManager" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetUserFullIdentity">
      <soap12:operation soapAction="http://tempuri.org/GetUserFullIdentity" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetUserEmail">
      <soap12:operation soapAction="http://tempuri.org/GetUserEmail" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="IsUserMemberOfSharePointGroup">
      <soap12:operation soapAction="http://tempuri.org/IsUserMemberOfSharePointGroup" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSharePointUsersByGroup">
      <soap12:operation soapAction="http://tempuri.org/Response1" style="document" />
      <wsdl:input name="Response1">
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output name="Response1">
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="DeleteNode">
      <soap12:operation soapAction="http://tempuri.org/DeleteNode" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="AppendXml">
      <soap12:operation soapAction="http://tempuri.org/AppendXml" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
  </wsdl:binding>
  <wsdl:service name="GweWebServices">
    <wsdl:port name="GweWebServicesSoap" binding="tns:GweWebServicesSoap">
      <soap:address location=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output); %> />
    </wsdl:port>
    <wsdl:port name="GweWebServicesSoap12" binding="tns:GweWebServicesSoap12">
      <soap12:address location=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output); %> />
    </wsdl:port>
  </wsdl:service>
</wsdl:definitions>