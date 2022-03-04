# bS files self generated for testing

this folder contains bS IDS files generated from the online service at:

https://www.liquid-technologies.com/online-xsd-to-xml-converter

To update the files with new versions:

1. Get the xsd
2. replace 
	2.1 minOccurs="0" with minOccurs="1"
	2.1 use="optional" with use="required"
3. Generate the xml sample.
4. Replace `<restriction />` with `<ids:simpleValue>string</ids:simpleValue>`
5. **Save as string values test file**
6. Alter the schema: Search `<xs:complexType name="idsValue">` element and remove the string option to generate constraints wherever possible
7. Generate the xml sample.
8. Replace `<restriction />` with `<xs:restriction base="xs:string"><xs:enumeration value="AlternativeOne"/><xs:enumeration value="AlternativeTwo"/></xs:restriction>`
9. Generate the xml sample, and **Save as restriction values test file**

XML root elements might have to be changed to `<ids:ids xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids.xsd" xmlns:ids="http://standards.buildingsmart.org/IDS">`
