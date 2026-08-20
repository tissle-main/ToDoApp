import { Service, signal } from "@angular/core";
import { UserDto } from "../../../api";

@Service()
export class UserStore
{
  public readonly user = signal<UserDto | null>(null);

  public setUser(user: UserDto)
  {
    this.user.set(user);
    localStorage.setItem("user", JSON.stringify(user));
  }
  public removeUser()
  {
    this.user.set(null);
    localStorage.removeItem("user");
  }
  public restoreUser()
  {
    const json: string | null = localStorage.getItem("user");
    if (json)
    {
      this.user.set(JSON.parse(json) as UserDto);
    }
  }
}
