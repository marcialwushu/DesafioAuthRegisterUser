import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  imagePath = '../../../assets/img/user-logo.png';

  constructor() { }

  ngOnInit() {
  }

}
