
Update the config as following: 

Find your the service behavior which you want to use for ClientCertificates. Add the lines below:

	<clientCertificate>
		<authentication certificateValidationMode="Custom" customCertificateValidatorType="Ced.Wcf.X509CertificatesValidator.CedX509CertificateValidator, Ced.Wcf.X509CertificatesValidator"/>
	</clientCertificate>

Find the allowedCertificates section:
Add the thumbprints necessary.