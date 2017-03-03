using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Public_Key_Encryptor {

  public partial class MainWindow : Window {
    private RSACryptoServiceProvider myRSA, otherRSA;

    public MainWindow() {
      InitializeComponent();
    }

    private void OnWindowLoaded( object sender, RoutedEventArgs e ) {
      myRSA = new RSACryptoServiceProvider();
      otherRSA = new RSACryptoServiceProvider();

      DisplayKeys();
    }

    private void OnLoadPrivateKeyClicked( object sender, RoutedEventArgs e ) {
      OpenFileDialog dialog = new OpenFileDialog();

      if ( dialog.ShowDialog() == true )
        myRSA.FromXmlString( File.ReadAllText( dialog.FileName ) );

      DisplayKeys();
    }

    private void OnSavePrivateKeyClicked( object sender, RoutedEventArgs e ) {
      SaveFileDialog dialog = new SaveFileDialog();

      if ( dialog.ShowDialog() == true )
        File.WriteAllText( dialog.FileName, myRSA.ToXmlString( true ) );
    }

    private void OnImportPublicKeyClicked( object sender, RoutedEventArgs e ) {
      OpenFileDialog dialog = new OpenFileDialog();

      if ( dialog.ShowDialog() == true )
        otherRSA.FromXmlString( File.ReadAllText( dialog.FileName ) );

      DisplayKeys();
    }

    // Remember: short messages due to RSA
    private void OnSaveEncryptedMessageClicked( object sender, RoutedEventArgs e ) {
      SaveFileDialog dialog = new SaveFileDialog();

      if ( dialog.ShowDialog() == true ) {
        byte[] plainText = Encoding.Unicode.GetBytes( messageTextBox.Text );
        byte[] cipherText = otherRSA.Encrypt( plainText, true );
        File.WriteAllBytes( dialog.FileName, cipherText );
      }
    }

    private void OnExportPublicKeyClicked( object sender, RoutedEventArgs e ) {
      SaveFileDialog dialog = new SaveFileDialog();

      if ( dialog.ShowDialog() == true )
        File.WriteAllText( dialog.FileName, myRSA.ToXmlString( false ) );
    }

    private void OnLoadEncryptedMessageClicked( object sender, RoutedEventArgs e ) {
      OpenFileDialog dialog = new OpenFileDialog();

      if ( dialog.ShowDialog() == true ) {
        byte[] cipherText = File.ReadAllBytes( dialog.FileName );
        byte[] plainText = myRSA.Decrypt( cipherText, true );
        messageTextBox.Text = Encoding.Unicode.GetString( plainText );
      }
    }

    private void DisplayKeys() {
      RSAParameters myRSAParameters = myRSA.ExportParameters( true );
      myPrivateKeyTextBox.Text = Convert.ToBase64String( myRSAParameters.D );
      myPublicKeyTextBox.Text = Convert.ToBase64String( myRSAParameters.Modulus );

      RSAParameters otherRSAParameters = otherRSA.ExportParameters( false );
      otherPublicKeyTextBox.Text = Convert.ToBase64String( otherRSAParameters.Modulus );
    }
  }
}
