import { computed, Service, signal } from "@angular/core";

@Service()
export class AuthTokenStore
{
  public readonly token = signal<string | null>(null);
  public readonly email = signal<string | null>(null);
  public readonly isAuthenticated = computed(() => this.token() !== null);

  public setToken(token: string, email: string)
  {
    this.token.set(token);
    this.email.set(email);
    localStorage.setItem("access_token", token);
    localStorage.setItem("email", email);
  }
  public removeToken()
  {
    this.token.set(null);
    this.email.set(null);
    localStorage.removeItem("access_token");
    localStorage.removeItem("email");
  }
  public restoreToken()
  {
    this.token.set(localStorage.getItem("access_token"));
    this.email.set(localStorage.getItem("email"));
  }
}
