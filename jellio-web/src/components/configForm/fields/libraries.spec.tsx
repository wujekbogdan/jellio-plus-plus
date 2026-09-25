import { cleanup, render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useForm } from 'react-hook-form';
import { afterEach, describe, expect, it } from 'vitest';
import { LibrariesField } from './libraries';
import type { ConfigFormType } from '@/components/configForm/formSchema';
import { Form } from '@/components/ui/form';

const movies = {
  key: 'aabbccdd11223344eeff00112233aabb',
  name: 'Movies',
  type: 'movies',
};
const shows = {
  key: '11223344556677889900aabbccddeeff',
  name: 'Shows',
  type: 'tvshows',
};

const Host = ({ defaultValues }: { defaultValues: ConfigFormType }) => {
  const form = useForm<ConfigFormType>({ defaultValues });
  return (
    <Form {...form}>
      <LibrariesField
        form={form}
        serverName="Home"
        libraries={[movies, shows]}
      />
    </Form>
  );
};

const sectionOf = (libraryName: string) =>
  within(screen.getByRole('group', { name: `${libraryName} | Home` }));

const enabledCheckbox = (libraryName: string) =>
  sectionOf(libraryName).getByRole('checkbox', { name: 'Enabled' });

const sortOrderSelect = (libraryName: string) =>
  sectionOf(libraryName).getByRole<HTMLSelectElement>('combobox', {
    name: 'Sort order',
  });

const sectionState = (libraryName: string) => ({
  enabled: enabledCheckbox(libraryName).getAttribute('aria-checked') === 'true',
  sortOrder: sortOrderSelect(libraryName).value,
  sortOrderDisabled: sortOrderSelect(libraryName).disabled,
});

afterEach(cleanup);

describe('LibrariesField', () => {
  it('should show each library as a section with its enabled state and sort order', () => {
    render(
      <Host
        defaultValues={{
          serverName: 'Home',
          libraries: [movies],
          sortOrders: {},
        }}
      />,
    );

    expect([sectionState('Movies'), sectionState('Shows')]).toEqual([
      { enabled: true, sortOrder: 'RecentlyAdded', sortOrderDisabled: false },
      { enabled: false, sortOrder: 'RecentlyAdded', sortOrderDisabled: true },
    ]);
  });

  it('should keep the chosen sort order when the library is turned off and on again', async () => {
    const user = userEvent.setup();
    render(
      <Host
        defaultValues={{
          serverName: 'Home',
          libraries: [movies],
          sortOrders: {},
        }}
      />,
    );

    await user.selectOptions(sortOrderSelect('Movies'), 'Release date');
    await user.click(enabledCheckbox('Movies'));
    await user.click(enabledCheckbox('Movies'));

    expect(sectionState('Movies')).toEqual({
      enabled: true,
      sortOrder: 'ReleaseDate',
      sortOrderDisabled: false,
    });
  });
});
