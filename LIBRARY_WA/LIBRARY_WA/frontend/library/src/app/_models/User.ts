export class User {

  userId: Number;
  login: String;
  password: String;
  userType: String;
  fullname: String;
  dateOfBirth: Date;
  phoneNumber: String;
  email: String;
  address: String;
  isValid: Boolean;

  constructor(
    userId: Number,
    login: String,
    password: String,
    userType: String,
    fullName: String,
    dateOfBirth: Date,
    phoneNumber: String,
    email: String,
    address: String,
    isValid: Boolean) {
  }
}
